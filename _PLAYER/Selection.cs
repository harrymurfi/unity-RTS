using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Selection : MonoBehaviour
{
	public event Action<Vector3> moveEvent;
	public event Action<Vector3> aggresiveMoveEvent;
	public event Action<Entitas> attackEvent;
	public event Action unitHoldEvent;
	public event Action unitStopEvent;
	public event Action cancelEvent;

	public event Action<Entitas> onRightSelectFriendlyUnit;
	public event Action<Building> onRightSelectFriendlyBuilding;
	public event Action<Entitas> onRightSelectEnemy;

	public static Selection instance { get; private set; }

	public enum MouseActionType {SingleClick, DoubleClick, DragClick}
	public MouseActionType mouseAction;
	public bool hasSelections;
	public LayerMask playerMask;

	// select Entity00
	public Dictionary<int, Entitas> allEntities = new Dictionary<int, Entitas>();
	public Dictionary<int, Entitas> OnScreenEntities = new Dictionary<int, Entitas>();
	public Dictionary<int, Entitas> selectedEntities = new Dictionary<int, Entitas>();

	// select ISelectable
	public List<ISelectable> playerEntities = new List<ISelectable>();
	public List<ISelectable> selectableEntities = new List<ISelectable>();
	public Dictionary<int, ISelectable> visibleEntities = new Dictionary<int, ISelectable>();

	public GameObject selectionBox;
	RectTransform boxRect;

	Player player;
	float timePressed, timeReleased;
	Vector3 positionPressed, positionReleased, positionCurrent, min, max;
	bool hasActiveBox;
	bool canSelect;
	RTSCamera cameraController;
	Bounds bound;

	public SelectionState currentState = SelectionState.Free;

	void Awake()
	{
		if(instance == null) instance = this;
		else Destroy(gameObject);
	}

	void Start()
	{
		player = GetComponent<Player>();
		boxRect = selectionBox.GetComponent<RectTransform>();
		selectionBox.SetActive(false);
		cameraController = FindObjectOfType<RTSCamera>();
	}

	void Update()
	{
		switch(currentState)
		{
		case SelectionState.Free:
			{
				if(Input.GetMouseButtonDown(0))
				{
					OnLeftMouseDown();
				}

				if(Input.GetMouseButton(0))
				{
					OnLeftMouseStay();
				}

				if(Input.GetMouseButtonUp(0))
				{
					OnLeftMouseUp();
				}

				if(Input.GetMouseButtonDown(1))
				{
					if(!EventSystem.current.IsPointerOverGameObject())
						SingleRightClick();
				}
				break;
			}
		case SelectionState.OnBuild:
			{
				if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					Cancelling();
				}
				break;
			}
		case SelectionState.OnAttackMode:
			{
				if(Input.GetMouseButtonDown(0))
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit, 200))
					{
						int layer = hit.collider.gameObject.layer;
						if(layer == 8)
						{
							SendAggresiveMove(hit.point);
						}
						else if(1 << layer == player.enemyMask)
						{
							SendAttack(hit.collider.GetComponent<Entitas>());
						}
					}
					Cancelling();
				}
				else if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
				{
					Cancelling();
				}
				break;
			}
		default: break;
		}

//		if(currentState == SelectionState.OnBuild)
//		{
//			if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
//			{
//				Cancelling();
//			}
//			return;
//		}
//		else if(currentState == SelectionState.OnAttackMode)
//		{
//			if(Input.GetMouseButtonDown(0))
//			{
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				RaycastHit hit;
//				if(Physics.Raycast(ray, out hit, 200))
//				{
//					int layer = hit.collider.gameObject.layer;
//					if(layer == 8)
//					{
//						SendAggresiveMove(hit.point);
//					}
//					else if(1 << layer == player.enemyMask)
//					{
//						SendAttack(hit.collider.GetComponent<Entitas>());
//					}
//				}
//			}
//		}
//		else if(currentState == SelectionState.Free)
//		{
//			if(Input.GetMouseButtonDown(0))
//			{
//				OnLeftMouseDown();
//			}
//			
//			if(Input.GetMouseButton(0))
//			{
//				OnLeftMouseStay();
//			}
//			
//			if(Input.GetMouseButtonUp(0))
//			{
//				OnLeftMouseUp();
//			}
//			
//			if(Input.GetMouseButtonDown(1))
//			{
//				if(!EventSystem.current.IsPointerOverGameObject())
//					SingleRightClick();
//			}
//		}

	}

	void OnLeftMouseDown()
	{
		if(EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		else
		{
			canSelect = true;
		}

		timePressed = Time.time;
		positionPressed = Input.mousePosition;
		if(timePressed - timeReleased < 0.2f && positionReleased == positionPressed) 
			mouseAction = MouseActionType.DoubleClick;
		else
			mouseAction = MouseActionType.SingleClick;

		CreateBoxSelection();
	}

	void OnLeftMouseStay()
	{
		if(Input.mousePosition != positionPressed) mouseAction = MouseActionType.DragClick;

		DragBoxSelection();
	}

	void OnLeftMouseUp()
	{
		if(!canSelect)
		{
			ResetSelectionBox();
			return;
		}

		timeReleased = Time.time;
		positionReleased = Input.mousePosition;
		ClearSelection();

		switch(mouseAction)
		{
		case MouseActionType.SingleClick:
			{
				SingleLeftClick();
				break;
			}
		case MouseActionType.DoubleClick:
			{
				DoubleLeftClick();
				break;
			}
		case MouseActionType.DragClick:
			{
				DragClick();
				break;
			}
		}

		mouseAction = MouseActionType.SingleClick;

		ResetSelectionBox();
	}

	// create box sequence
	void CreateBoxSelection()
	{
		hasActiveBox = true;
		selectionBox.SetActive(true);
		boxRect.anchoredPosition3D = positionPressed;
	}

	void DragBoxSelection()
	{
		if(hasActiveBox)
		{
			cameraController.canScrollView = false;
			positionCurrent = Input.mousePosition;
			min = Vector3.Min(positionPressed, positionCurrent);
			max = Vector3.Max(positionPressed, positionCurrent);

			Vector2 newSize = max - min;
			boxRect.anchoredPosition3D = min;
			boxRect.sizeDelta = newSize;
		}
	}

	void ResetSelectionBox()
	{
		cameraController.canScrollView = true;
		selectionBox.SetActive(false);
		hasActiveBox = false;
		canSelect = false;
	}

	// selection type
	void SingleLeftClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100, playerMask))
		{
			Entitas entity = hit.collider.GetComponent<Entitas>();
			entity.OnSelected();
			selectedEntities.Add(entity.instanceID, entity);
			if(entity is Building)
			{
				Building b = (Building) entity;
				if(b.currentState != BuildingState.Complete)
				{
					UIController.instance.commandController.Create("Incomplete Building");
				}
				else
				{
					UIController.instance.commandController.Create(b.entityName);
				}
			}
			else
			{
				UIController.instance.commandController.Create(entity.entityName);
			}
		}
	}

	void SingleRightClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100))
		{
			if(hit.collider.gameObject.layer == 8)
			{
				SendMove(hit.point);
			}
			else if(1 << hit.collider.gameObject.layer == player.playerMask)
			{
				Entitas entity = hit.collider.gameObject.GetComponent<Entitas>();
				if(entity.entityType == EntitasType.Building)
				{
					print("go to building");
					Building building = (Building) entity;
					if(building.currentState != BuildingState.Complete && building.builder == null)
					{
						if(onRightSelectFriendlyBuilding != null) onRightSelectFriendlyBuilding(building);
					}
				}
				else if(entity.entityType == EntitasType.Unit)
				{
					print("go to that unit");
				}
			}
			else if(1 << hit.collider.gameObject.layer == player.enemyMask) 
			{
				Entitas e = hit.collider.gameObject.GetComponent<Entitas>();
				SendAttack(e);
			}
		}
	}

	void DoubleLeftClick()
	{

	}

	void DragClick()
	{
		AddWithinBounds();
//		if(selectableEntities.Count == 0) return;
//		if(selectableEntities.Count == 1) 
//		{
//			//UIController00.Instance.UpdateInfoPanel(selectableEntities[0].ISelectID);
//			UIController00.instance.commandController.Create(selectableEntities[0].ISelectID);
//		}
//		else
//		{
//			UIController00.instance.commandController.Create(999);
//		}
	}

	// unit selection
	void AddWithinBounds()
	{
		bound = GetViewportBounds(Camera.main, positionPressed, positionReleased);
//		foreach(ISelectable entity in visibleEntities.Values)
//		{
//			if(IsWithinBounds(Camera.main, bound, entity.ISelectTransform.localPosition))
//			{
//				if(!selectableEntities.Contains(entity))
//				{
//					selectableEntities.Add(entity);
//					entity.OnSelected();
//				}
//			}
//		}

		// first phase - select all
		foreach(KeyValuePair<int, Entitas> entitas in OnScreenEntities)
		{
			if(IsWithinBounds(Camera.main, bound, entitas.Value.transform.localPosition))
			{
				if(!selectedEntities.ContainsKey(entitas.Key))
				{
					selectedEntities.Add(entitas.Key, entitas.Value);
					//entitas.Value.OnSelected();
				}
			}
		}


		// second phase - filtering
		if(selectedEntities.Count == 0)
		{
			return;
		}
		else if(selectedEntities.Count == 1) 
		{
			Entitas entity = selectedEntities.Values.First();
			entity.OnSelected();
			if(entity is Building)
			{
				Building b = (Building) entity;
				if(b.currentState != BuildingState.Complete)
				{
					UIController.instance.commandController.Create("Incomplete Building");
				}
				else
				{
					UIController.instance.commandController.Create(b.entityName);
				}
			}
			else
			{
				UIController.instance.commandController.Create(entity.entityName);
			}
		}
		else
		{
			// check if any building mixed with units
			List<int> toRemove = selectedEntities.Where(pair => pair.Value.entityType == EntitasType.Building).Select(pair => pair.Key).ToList();
			foreach(int i in toRemove)
			{
				selectedEntities.Remove(i);
			}

			foreach(KeyValuePair<int, Entitas> pair in selectedEntities)
			{
				pair.Value.OnSelected();
			}

			if(selectedEntities.Count == 1) UIController.instance.commandController.Create(selectedEntities.Values.First().entityName);
			else UIController.instance.commandController.Create("Generic");
		}
	}

	void ClearSelection()
	{
		// ISelectable
//		if(selectableEntities.Count > 0)
//		{
//			foreach(ISelectable entity in selectableEntities)
//			{
//				entity.OnDeselected();
//			}
//
//			selectableEntities.Clear();
//		}

		// Entitas
		if(selectedEntities.Count > 0)
		{
			foreach(KeyValuePair<int, Entitas> entitas in selectedEntities)
			{
				entitas.Value.OnDeselected();
			}

			selectedEntities.Clear();
		}

		// update for ui controller
		UIController.instance.commandController.Clear();
	}

	// unit action
	void SendMove(Vector3 destination)
	{
		if(moveEvent != null) moveEvent(destination);
	}

	void SendAggresiveMove(Vector3 destination)
	{
		if(aggresiveMoveEvent != null) aggresiveMoveEvent(destination);
	}

	void SendAttack(Entitas target)
	{
		if(attackEvent != null) attackEvent(target);
	}

	public void SendHold()
	{
		if(unitHoldEvent != null) unitHoldEvent();
	}

	public void Cancelling()
	{
		if(cancelEvent != null) cancelEvent();
		ChangeState(SelectionState.Free);
	}

	// utility
	public void ChangeState(SelectionState state)
	{
		switch(state)
		{
		case SelectionState.Free:
			{
				UIController.instance.ChangeCursor("normal");
				currentState = SelectionState.Free;
				break;
			}
		case SelectionState.OnAttackMode:
			{
				UIController.instance.ChangeCursor("attack");
				currentState = SelectionState.OnAttackMode;
				break;
			}
		}
	}

	Bounds GetViewportBounds(Camera camera, Vector3 inputStart, Vector3 inputEnd)
	{
		Vector3 inputStartView = camera.ScreenToViewportPoint(inputStart);
		Vector3 inputEndView = camera.ScreenToViewportPoint(inputEnd);
		Vector3 min = Vector3.Min(inputStartView, inputEndView);
		Vector3 max = Vector3.Max(inputStartView, inputEndView);

		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		Bounds bound = new Bounds();
		bound.SetMinMax(min, max);
		return bound;
	}

	bool IsWithinBounds(Camera camera, Bounds viewportBounds, Vector3 position)
	{
		Vector3 viewportPoint = camera.WorldToViewportPoint(position);
		return viewportBounds.Contains(viewportPoint);
	}

	int GetInfoRaycast()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 300))
		{
			return hit.collider.gameObject.layer;
		}
		else
		{
			return -1;
		}
	}
}