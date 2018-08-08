using UnityEngine;

public class ResourceField : Building
{
	static ResourceManager RM;
	public ResourceType type;
	public int production = 3;
	int multiplier = 2;

	protected override void Start()
	{
		base.Start();
		if(RM == null)
		{
			RM = FindObjectOfType<ResourceManager>();
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void ToComplete()
	{
		base.ToComplete();
		RM.UpdateRate(type, production);
	}

	protected override void Complete()
	{
		base.Complete();
	}

	public override void Upgrade()
	{
		base.Upgrade();
		print("resource field upgrade");
	}

	void OnCollisionStay(Collision collisionInfo)
	{
		print("col stay");
	}
}
