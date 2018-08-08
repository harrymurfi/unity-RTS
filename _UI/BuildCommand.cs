using UnityEngine;
using UnityEngine.UI;

public class BuildCommand : Command
{
	Button btn;
	ResourceInfo cost;
	bool costSufficient;

	void Start()
	{
		//this.tooltips = name;
		if(Utils.buildingCostTable.ContainsKey(name))
		{
			cost = Utils.buildingCostTable[name];
		}
		else
		{
			print("No cost for " + name + " yet.");
		}
		costSufficient = ResourceManager.instance.resources.IsSufficient(cost);
		btn = GetComponent<Button>();
		btn.interactable = costSufficient;
		btn.onClick.AddListener(OnButtonClick);
		ResourceManager.ResourceUpdate += OnResourceUpdate;
	}

	void OnButtonClick()
	{
		if(!costSufficient)
		{
			AudioController.instance.PlaySingle("insufficient funds");
			return;
		}
		else
		{
			BuildManager.instance.GhostBuilding(name);
			CommandController.instance.ExtentCommand("Cancel");
		}
	}

	public void OnResourceUpdate()
	{
		costSufficient = ResourceManager.instance.resources.IsSufficient(cost);
		btn.interactable = costSufficient;
	}

	void OnDestroy()
	{
		ResourceManager.ResourceUpdate -= OnResourceUpdate;
	}
}
