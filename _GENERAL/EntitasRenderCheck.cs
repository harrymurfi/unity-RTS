using UnityEngine;

public class EntitasRenderCheck : MonoBehaviour
{
	public Entitas entity;

	void OnBecameVisible()
	{
		if(!Selection.instance.OnScreenEntities.ContainsKey(entity.instanceID))
			Selection.instance.OnScreenEntities.Add(entity.instanceID, entity);
	}

	void OnBecameInvisible()
	{
		if(Selection.instance.OnScreenEntities.ContainsKey(entity.instanceID))
			Selection.instance.OnScreenEntities.Remove(entity.instanceID);
	}
}
