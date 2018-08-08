using UnityEngine;

public class Granary : Building
{
	protected override void ToComplete()
	{
		base.ToComplete();
		UpgradeGranaryCapacity();
	}

	void UpgradeGranaryCapacity()
	{
		ResourceManager.instance.GranaryCap += 400 * level;
	}
}
