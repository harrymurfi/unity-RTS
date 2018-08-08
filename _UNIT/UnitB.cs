using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitB : Entitas
{
	static int runHash = Animator.StringToHash("Run");
	static int attackHash = Animator.StringToHash("Attack");
	static int deadHash = Animator.StringToHash("Dead");

	Animator anim;
	NavMeshAgent agent;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	Image hpbarImage;
	HPBar hpBar;

	protected Entitas enemy;
	Collider[] enemyCollider = new Collider[1];
	public int visibilityRange;

	public UnitState currentState;

	protected virtual void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		anim = GetComponentInChildren<Animator>();
		agent= GetComponent<NavMeshAgent>();
		agent.enabled = false;
		obstacle = GetComponent<NavMeshObstacle>();

		// UI related
		hpBar = GetComponentInChildren<HPBar>();
		hpBar.anchor = transform;
		hpbarImage = hpBar.GetComponent<Image>();
		hpbarImage.enabled = false;
		hpBar.transform.SetParent(UIController.instance.attributesContainer.transform, false);
		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;

		StandbyTransition();
	}

	protected virtual void Update()
	{
		switch(currentState)
		{
		case UnitState.Standby:
			{
				break;
			}
		case UnitState.Hold:
			{
				break;
			}
		case UnitState.Move:
			{
				break;
			}
		case UnitState.Attack:
			{
				break;
			}
		case UnitState.Dead:
			{
				break;
			}
		}
	}

	protected virtual void StandbyTransition()
	{
		StopCoroutine("SphereCheck");
		StartCoroutine("SphereCheck");
		currentState = UnitState.Standby;
	}

	protected virtual void StandbyState()
	{
		if(enemy != null)
		{
			
		}
	}

	IEnumerator SphereCheck()
	{
		int range = 1;
		while(enemy == null)
		{
			if(Physics.OverlapSphereNonAlloc(transform.position, range, enemyCollider, player.enemyMask) > 0)
			{
				enemy = enemyCollider[0].GetComponent<Entitas>();
				break;
			}
			else
			{
				if(range < visibilityRange)
				{
					range += 2;
					print("enemy not found 0.1s");
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				else
				{
					print("enemy not found 0.5s");
					yield return new WaitForSeconds(0.5f);
				}
			}
		}
		print("enemy found");
	}

	public override void OnSelected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
			//Selection.instance.moveEvent += Move;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
			//Selection.instance.moveEvent -= Move;
		}
	}
}
