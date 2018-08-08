using UnityEngine;

public class RenderCheck : MonoBehaviour
{
	ISelectable entity;

	void Start()
	{
		entity = GetComponentInParent<ISelectable>();
	}

	void OnBecameVisible()
	{
		if(Selection.instance.visibleEntities.ContainsKey(entity.InstanceID))
		{
			print("duplicate key exist for " + entity.InstanceID.ToString());
			return;
		}
		else
		{
			Selection.instance.visibleEntities.Add(entity.InstanceID, entity);
		}
	}

	void OnBecameInvisible()
	{
		if(Selection.instance.visibleEntities.ContainsKey(entity.InstanceID))
		{
			Selection.instance.visibleEntities.Remove(entity.InstanceID);
			return;
		}
		else
		{
			print("invalid key not exist for " + entity.InstanceID.ToString());
		}
	}
}
