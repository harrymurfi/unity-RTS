using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class CommandController : MonoBehaviour
{
	public static CommandController instance;
	public List<GameObject> tempPrefabs;

	Dictionary<string, GameObject> commandPanelList = new Dictionary<string, GameObject>();
	GameObject current;
	GameObject last;
	string currentCommand;
	string lastCommand;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		ArrayToDictionary();
	}

	public void Create(string key)
	{
		if(!commandPanelList.ContainsKey(key))
		{
			Debug.Log("No Command Panel for " + key);
			return;
		}
//		else if(key == 2)
//		{
//			
//		}
		else
		{
			Clear();
			currentCommand = key;
			current = Instantiate(commandPanelList[key]);
			current.transform.SetParent(transform, false);
		}
	}

	public void Clear()
	{
		if(current != null)
		{
			lastCommand = currentCommand;
			Destroy(current.gameObject);
		}
	}

	void ArrayToDictionary()
	{
		foreach(GameObject go in tempPrefabs)
		{
			commandPanelList.Add(go.name, go);
		}
	}

	// Commands
	public void TrainCommand(string identifier)
	{
		if(ResourceManager.instance.resources.IsSufficient(Utils.unitCostTable[identifier]))
		{
			AudioController.instance.PlaySingle("training");
			ResourceManager.instance.resources.Reduce(Utils.unitCostTable[identifier]);
			ITrainable trainer = Selection.instance.selectedEntities.Values.First().GetComponent<ITrainable>();
			trainer.Train(identifier);
		}
		else
		{
			AudioController.instance.PlaySingle("insufficient funds");
		}
//		if(identifier == "Villager")
//		{
//			MainBuilding building = SelectionController00.instance.selectedEntities.Values.First().GetComponent<MainBuilding>();
//			building.AddTrainingQueueu();
//		}
	}

	public void ExtentCommand(string identifier)
	{
		Clear();
		Create(identifier);
	}

	public void CancelCommand()
	{
		Create(lastCommand);
	}

	public void UpgradeBuilding()
	{
		if(Selection.instance.selectedEntities.Values.First().entityType != EntitasType.Building)
		{
			print("Command upgrade failed. Entity in selection not a building");
		}
		else
		{
			Building building = Selection.instance.selectedEntities.Values.First().GetComponent<Building>();
			building.Upgrade();
		}
	}

	public void AttackCommand()
	{
		
	}
}
