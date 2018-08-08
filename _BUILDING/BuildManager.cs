using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
	public static BuildManager instance { get; private set; }

	public GameObject[] buildingArray;
	public GameObject[] ghostBuildingArray;
	public Dictionary<string, GameObject> buildingList = new Dictionary<string, GameObject>();
	public Dictionary<string, GameObject> ghostBuildingList = new Dictionary<string, GameObject>();

	public bool buildMode;

	void Awake()
	{
		if(instance == null) instance = this;
		else Destroy(gameObject);
	}

	void Start()
	{
		for(int i = 0; i < buildingArray.Length; i++)
		{
			Entitas e = buildingArray[i].GetComponent<Entitas>();
			buildingList.Add(e.entityName, buildingArray[i]);
		}

		for(int i = 0; i < ghostBuildingArray.Length; i++)
		{
			GhostBuilding gb = ghostBuildingArray[i].GetComponent<GhostBuilding>();
			ghostBuildingList.Add(gb.Bname, ghostBuildingArray[i]);
		}

		buildingArray = null;
		ghostBuildingArray = null;
	}

	public void Build(string name)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 200, 1 << 8))
		{
			if(buildingList.ContainsKey(name))
			{
				//print("reduce cost for " + name);
				ResourceManager.instance.resources.Reduce(Utils.buildingCostTable[name]);
				GameObject go = Instantiate(buildingList[name], hit.point, Utils.standardRotation) as GameObject;
				Building b = go.GetComponent<Building>();
				b.builder = Selection.instance.selectedEntities.Values.First();
			}
			else
			{
				print("Building prefab no available.");
			}
		}
	}

	public void Build(string name, Vector3 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 200, 1 << 8))
		{
			if(buildingList.ContainsKey(name))
			{
				//print("reduce cost for " + name);
				ResourceManager.instance.resources.Reduce(Utils.buildingCostTable[name]);
				GameObject go = Instantiate(buildingList[name], position, Utils.standardRotation) as GameObject;
				Building b = go.GetComponent<Building>();
				b.builder = Selection.instance.selectedEntities.Values.First();
			}
			else
			{
				print("Building prefab no available.");
			}
		}
	}

	public void GhostBuilding(string name)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 200, 1 << 8))
		{
			if(ghostBuildingList.ContainsKey(name))
			{
				GameObject go = Instantiate(ghostBuildingList[name], hit.point, Utils.standardRotation) as GameObject;
				if(Selection.instance.currentState == SelectionState.Free)
					Selection.instance.currentState = SelectionState.OnBuild;
			}
			else
			{
				print("No available.");
			}
		}
	}
}
