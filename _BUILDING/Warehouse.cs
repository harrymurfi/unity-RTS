using UnityEngine;

public class Warehouse : Building
{
	protected override void ToComplete()
	{
		base.ToComplete();
		UpgradeWarehouseCapacity();
	}

	void UpgradeWarehouseCapacity()
	{
		ResourceManager.instance.WarehouseCap += 400 * level;
	}
}
