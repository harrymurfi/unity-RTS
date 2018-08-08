using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Entity : MonoBehaviour {

	public event Action OnDeath;

	public PlayerController player;
	public bool isSelected;

	Team team;

	bool damageable = true;
	protected bool destroyed;

	public float initHitpoints;
	protected float curHitpoints;

	public Texture2D hitpointBar;
	Renderer rend;

	protected virtual void Start()
	{
		team = player.playerTeam;
		curHitpoints = initHitpoints;
		rend = GetComponentInChildren<Renderer>();
		ChangeSelection(false);

		SetLayer(this.team);

		if(!player.playerEntities.Contains(this))
		{
			player.playerEntities.Add(this);
		}
	}

	protected virtual void Update()
	{
		
	}

	protected virtual void FixedUpdate()
	{
		VisibilityCheck();
	}

	protected void TakenDamage(float amount)
	{
		if(damageable)
		{
			curHitpoints -= amount;
		}
	}

	protected virtual void OnGUI()
	{
		if(isSelected && rend.isVisible)
		{
			DrawHealthBar(Camera.main.WorldToScreenPoint(transform.position));
		}
	}

	void DrawHealthBar(Vector3 position)
	{
		float hpBarWidth = (curHitpoints/initHitpoints) * 30;
		Rect hpBarRect = new Rect(position.x - 15, Screen.height - position.y + 20, hpBarWidth, 5);
		GUI.DrawTexture(hpBarRect, hitpointBar);
	}

	void SetLayer(Team team)
	{
		switch(team)
		{
		case Team.Red : gameObject.layer = 11; break;
		case Team.Blue : gameObject.layer = 12; break;
		case Team.Green : gameObject.layer = 13; break;
		case Team.Yellow : gameObject.layer = 14; break;
		}
	}

	void VisibilityCheck()
	{
		if(!destroyed)
		{
			if(rend.isVisible)
			{
				if(!player.visibleEntities.Contains(this))
					player.visibleEntities.Add(this);
			}
			else
				player.visibleEntities.Remove(this);
		}
	}

	public void ChangeSelection(bool condition)
	{
		isSelected = condition;
	}

//	void OnDrawGizmos() {
//		Vector3 center = rend.bounds.center;
//		Vector3 size = rend.bounds.size;
//		Gizmos.color = Color.white;
//		Gizmos.DrawWireCube(center, size);
//	}
}
