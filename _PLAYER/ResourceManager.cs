using UnityEngine;
using System;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager instance;
	public static event Action ResourceUpdate;

	public ResourceInfo resources = new ResourceInfo(800, 800, 800, 800);

	public int woodRate;
	public int clayRate;
	public int ironRate;
	public int cropRate;

	int warehouseCap = 800;
	int granaryCap = 800;

	public int WarehouseCap
	{
		get { return this.warehouseCap; }
		set
		{
			this.warehouseCap = value;
			UIController.instance.wareHouseCap = "/" + warehouseCap.ToString();
		}
	}

	public int GranaryCap
	{
		get { return this.granaryCap; }
		set
		{
			this.granaryCap = value;
			UIController.instance.granaryCap= "/" + granaryCap.ToString();
		}
	}

	public bool couldUpdate = true;

	void Awake()
	{
		if(instance == null) instance = this;
	}

	void Start()
	{
		StartCoroutine(UpdateResources());
	}

	IEnumerator UpdateResources()
	{
		while(couldUpdate)
		{
			resources.wood += woodRate;
			resources.clay += clayRate;
			resources.iron += ironRate;
			resources.crop += cropRate;
			UIController.instance.UpdateProductionHud(resources);
			if(ResourceUpdate != null) ResourceUpdate();
			yield return new WaitForSeconds(1f);
		}
	}

	public void UpdateRate(ResourceType type, int amount)
	{
		switch(type)
		{
		case ResourceType.Wood: 
			{
				woodRate += amount;
				UIController.instance.UpdateProductionRateHud(type, woodRate);
				break;
			}
		case ResourceType.Clay:
			{
				clayRate += amount;
				UIController.instance.UpdateProductionRateHud(type, clayRate);
				break;
			}
		case ResourceType.Iron:
			{
				ironRate += amount;
				UIController.instance.UpdateProductionRateHud(type, ironRate);
				break;
			}
		case ResourceType.Crop:
			{
				cropRate += amount;
				UIController.instance.UpdateProductionRateHud(type, cropRate);
				break;
			}
		}
	}
}

[Serializable]
public struct ResourceInfo
{
	public int wood;
	public int clay;
	public int iron;
	public int crop;

	public ResourceInfo(int wood, int clay, int iron, int crop)
	{
		this.wood = wood;
		this.clay = clay;
		this.iron = iron;
		this.crop = crop;
	}

	public bool IsSufficient(ResourceInfo resources)
	{
		if(this.wood - resources.wood < 0) return false;
		if(this.clay - resources.clay < 0) return false;
		if(this.iron - resources.iron < 0) return false;
		if(this.crop - resources.crop < 0) return false;

		return true;
	}

	public void Reduce(ResourceInfo cost)
	{
		this.wood -= cost.wood;
		this.clay -= cost.clay;
		this.iron -= cost.iron;
		this.crop -= cost.crop;
	}
}
