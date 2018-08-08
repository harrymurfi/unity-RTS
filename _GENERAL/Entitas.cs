using UnityEngine;
using UnityEngine.UI;
using System;

public class Entitas : MonoBehaviour
{
	public Player player;
	public int entityID;
	public string entityName;
	public EntitasType entityType;
	[HideInInspector]
	public int instanceID;

	public float maxHealth;
	public float healthPoint;

	public event Action deathEvent;

	protected virtual void Start()
	{
		healthPoint = maxHealth;
		player = FindObjectOfType<Player>();
	}

	public virtual void TakeDamage(float damage)
	{
		healthPoint -= damage;
	}

	protected void Die()
	{
		if(Selection.instance != null)
		{
			if(Selection.instance.OnScreenEntities.ContainsKey(instanceID))
				Selection.instance.OnScreenEntities.Remove(instanceID);

			if(Selection.instance.selectedEntities.ContainsKey(instanceID))
				Selection.instance.selectedEntities.Remove(instanceID);
		}
		if(deathEvent != null) deathEvent();
		Destroy(gameObject, 10f);
	}

	public virtual void OnSelected()
	{
		print("selected");
	}

	public virtual void OnDeselected()
	{
		print("deselected");
	}
}

public enum EntitasType
{
	Undefine,
	Unit,
	Building,
}
