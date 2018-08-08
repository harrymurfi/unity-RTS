using UnityEngine;
using UnityEngine.EventSystems;

public class GhostBuilding : MonoBehaviour
{
	public string Bname;

	Renderer mainRend;
	bool invalidLocation;

	static Color ghostColor = new Color(0, 1, 0, 0.25f);
	bool onTrigger;

	void Start()
	{
		mainRend = GetComponentInChildren<Renderer>();
		Selection.instance.cancelEvent += OnCancelBuild;
	}

	void Update()
	{
		if(EventSystem.current.IsPointerOverGameObject())
		{
			if(Input.GetMouseButtonDown(0))
			{
				print("Cancelling");
				Selection.instance.Cancelling();
			}
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100, 1 << 8))
		{
			Vector3 snapPosition = new Vector3(Mathf.RoundToInt(hit.point.x), hit.point.y, Mathf.RoundToInt(hit.point.z));
			transform.position = snapPosition;
		}

		if(Input.GetMouseButtonDown(0))
		{
			if(invalidLocation) 
			{
				
			}
			else
			{
				BuildManager.instance.Build(Bname, transform.position);
				Destroy(gameObject);
			}
		}
	}

	void OnDestroy()
	{
		Selection.instance.cancelEvent -= OnCancelBuild;
	}

	public void OnCancelBuild()
	{
		Destroy(gameObject);
	}

	void OnTriggerStay()
	{
		if(!onTrigger)
		{
			invalidLocation = true;
			foreach(Material mat in mainRend.materials)
			{
				mat.color = Utils.invalidColor;
			}
			onTrigger = true;
		}
	}

	void OnTriggerExit()
	{
		invalidLocation = false;
		foreach(Material mat in mainRend.materials)
		{
			mat.color = Utils.greenGhostColor;
		}
		onTrigger = false;
	}
}
