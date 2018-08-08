using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Building : Entitas
{
	public event Action OnBuildComplete;

	Renderer mainRend;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	Image hpbarImage;
	HPBar hpBar;
	BuildTimeBar buildBar;
	[HideInInspector]
	public Transform spawnPoint;

	public BuildingState currentState;
	Vector3 anchor;
	bool invalidLocation;
	public float buildTime;
	float startBuildTime;
	public float percentBuilt;

	public Entitas builder;
	public bool isBuilderExist;

//	public ResourceInfo buildCost;
	public int level = 1;
	Color mainColor;

	protected override void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		mainRend = GetComponentInChildren<Renderer>();
		obstacle = GetComponent<NavMeshObstacle>();
		mainColor = mainRend.material.color;

		// UI related
		hpBar = GetComponentInChildren<HPBar>();
		hpBar.anchor = transform;
		hpbarImage = hpBar.GetComponent<Image>();
		hpbarImage.enabled = false;
		hpBar.transform.SetParent(UIController.instance.attributesContainer.transform, false);
		buildBar = GetComponentInChildren<BuildTimeBar>();
		buildBar.anchor = transform;
		buildBar.transform.SetParent(UIController.instance.attributesContainer.transform, false);
		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;
		spawnPoint = transform.Find("_spawn_point");

		if(currentState == BuildingState.Complete) ToComplete();
		else ToPrepare();
	}

	protected virtual void Update()
	{
		switch(currentState)
		{
		//case BuildingState.Hover: Hover(); break;
		case BuildingState.Prepare: Prepare(); break;
		case BuildingState.Construct: Construct(); break;
		case BuildingState.Complete: Complete(); break;
		}
	}

//	void ToHover()
//	{
//		if(SelectionController00.instance.currentState == SelectionState.Free)
//			SelectionController00.instance.currentState = SelectionState.OnBuild;
//		mainColor.a = 0.5f;
//		mainRend.material.color = mainColor;
//		currentState = BuildingState.Hover;
//	}
//
//	void Hover()
//	{
//		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
//		{
//			if(SelectionController00.instance.currentState == SelectionState.OnBuild)
//				SelectionController00.instance.currentState = SelectionState.Free;
//			print("build cancel");
//			Destroy(gameObject);
//		}
//
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		RaycastHit hit;
//		if(Physics.Raycast(ray, out hit, 100, 1 << 8))
//		{
//			Vector3 snapPosition = new Vector3(Mathf.RoundToInt(hit.point.x), hit.point.y, Mathf.RoundToInt(hit.point.z));
//			transform.position = snapPosition;
//		}
//
//		if(Input.GetMouseButtonDown(0) && !invalidLocation)
//		{
//			ToPrepare();
//		}
//	}

	void ToPrepare()
	{
		if(Selection.instance.currentState == SelectionState.OnBuild)
		{
			Selection.instance.currentState = SelectionState.Free;
			//Destroy(SelectionController00.instance.selectableEntities[0].ISelectTransform.gameObject);
			Selection.instance.selectableEntities.Clear();
			UIController.instance.commandController.Clear();
		}

		if(!isBuilderExist)
		{
			if(builder is VillagerCart)
			{
				VillagerCart cart = (VillagerCart) builder;
				cart.Build(this);
			}
			if(builder is Villager)
			{
				Villager villager = (Villager) builder;
				villager.Build(this);
			}
		}

		mainRend.material.color = Color.grey;
		obstacle.enabled = true;
		currentState = BuildingState.Prepare;
	}

	void Prepare()
	{
		if(isBuilderExist) ToConstruct();
	}

	void ToConstruct()
	{
//		mainColor.a = .75f;
//		mainRend.material.color = mainColor;
		obstacle.enabled = true;
		startBuildTime = buildTime;
		currentState = BuildingState.Construct;
	}

	void Construct()
	{
		if(isBuilderExist)
		{
			if(startBuildTime > 0)
			{
				startBuildTime -= Time.deltaTime;
				percentBuilt = (buildTime - startBuildTime) / buildTime;
				buildBar.ChangeScale(percentBuilt);
			}
			else
			{
				ToComplete();
			}

//			if(percentBuilt <= 1f)
//			{
//				if(Time.time > startBuildTime)
//				{
//					percentBuilt += 0.1f;
//					startBuildTime += (0.1f * buildTime);
//					buildBar.ChangeScale(percentBuilt);
//				}
//			}
//			else
//			{
//				ToComplete();
//			}
		}
	}

	protected virtual void ToComplete()
	{
		if(OnBuildComplete != null) OnBuildComplete();

		Destroy(buildBar.gameObject, 0.1f);
		mainColor.a = 1f;
		mainRend.material.color = mainColor;
		obstacle.enabled = true;
		currentState = BuildingState.Complete;
	}

	protected virtual void Complete()
	{
		
	}

	public virtual void Upgrade()
	{
		print("building upgrade");
	}

	public void AssignBuilder(Entitas entity)
	{
		builder = entity;
		isBuilderExist = true;
	}

	public void DeassignBuilder()
	{
		builder = null;
		isBuilderExist = false;
	}

	public override void OnSelected()
	{
		if(currentState != BuildingState.Destroyed)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != BuildingState.Destroyed)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
		}
	}

//	void OnTriggerStay(Collider other)
//	{
//		if(currentState == BuildingState.Hover)
//		{
//			invalidLocation = true;
//			mainRend.material.color = invalidColor;
//		}
//	}
//
//	void OnTriggerExit()
//	{
//		if(currentState == BuildingState.Hover)
//		{
//			invalidLocation = false;
//			mainRend.material.color = mainColor;
//		}
//	}
}
