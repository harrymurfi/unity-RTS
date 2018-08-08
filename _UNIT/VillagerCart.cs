using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VillagerCart : Entitas
{
	static int moveHash = Animator.StringToHash("Move");
	static int deadHash = Animator.StringToHash("Dead");

	Animator anim;
	NavMeshAgent agent;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	public Image hpbarImage;
	public HPBar hpBar;
	public UnitState currentState = UnitState.Idle;

	Vector3 destination;

	public LayerMask enemyMask;
	static float visibilityRange = 10f;
	Building workplace;

	public NavMeshPathStatus status;
	public float distremain;

	protected override void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		anim = GetComponentInChildren<Animator>();
		agent= GetComponent<NavMeshAgent>();
		agent.enabled = false;
		obstacle = GetComponent<NavMeshObstacle>();
		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;
		hpbarImage.enabled = false;
		hpBar = hpbarImage.GetComponent<HPBar>();
	}

	void Update()
	{
		if(agent.enabled)
		{
			status = agent.pathStatus;
			distremain = agent.remainingDistance;
		}
		switch(currentState)
		{
		case UnitState.Idle: IdleState(); break;
		case UnitState.Move: MoveState(); break;
		case UnitState.Build: BuildState(); break;
		case UnitState.Dead: DeadState(); break;
		}
	}

	void IdleTransition()
	{
		agent.enabled = false;
		obstacle.enabled = true;
		anim.SetBool(moveHash, false);
		currentState = UnitState.Idle;
	}

	void IdleState()
	{
		
	}

	void MoveTransition()
	{
		anim.SetBool(moveHash, true);
		StopAllCoroutines();
		StartCoroutine(Moving(destination, 1f));
		currentState = UnitState.Move;
	}

	void MoveState()
	{
		if(agent.enabled)
		{
			if(!agent.pathPending)
			{
				if(agent.remainingDistance <= agent.stoppingDistance)
				{
					IdleTransition();
				}
			}
		}
	}

	void BuildTransition()
	{
		anim.SetBool(moveHash, true);
		StopAllCoroutines();
		StartCoroutine(Moving(workplace.spawnPoint.transform.position, 2f));
		currentState = UnitState.Build;
	}

	void BuildState()
	{
		if(agent.enabled)
		{
			if(!agent.pathPending)
			{
				if(agent.remainingDistance <= agent.stoppingDistance)
				{
					workplace.AssignBuilder(this);
					OnDeselected();
					if(Selection.instance != null)
					{
						if(Selection.instance.OnScreenEntities.ContainsKey(instanceID))
							Selection.instance.OnScreenEntities.Remove(instanceID);

						if(Selection.instance.selectedEntities.ContainsKey(instanceID))
							Selection.instance.selectedEntities.Remove(instanceID);
					}
					Destroy(hpBar.gameObject);
					Destroy(gameObject);
				}
			}
		}
	}

	void DeadTransition()
	{
		gameObject.layer = 0;
		obstacle.enabled = false;
		agent.enabled = false;
		anim.SetTrigger(deadHash);
		Selection.instance.moveEvent -= Move;
		Destroy(hpBar.gameObject);
		Die();
		currentState = UnitState.Dead;
	}

	void DeadState()
	{

	}

	IEnumerator Moving(Vector3 dest, float stoppingDist)
	{
		obstacle.enabled = false;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		agent.enabled = true;
		agent.stoppingDistance = stoppingDist;
		agent.SetDestination(dest);
	}

	void RotateToTarget(Transform target)
	{
		Vector3 dir = (target.position - transform.position).normalized;
		dir = new Vector3(dir.x, 0, dir.z);
		Quaternion look = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, look, 10 * Time.deltaTime);
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		hpBar.ChangeHitbarScale(healthPoint / maxHealth);
		if(healthPoint <= 0)
		{
			DeadTransition();
		}
	}

	public void Move(Vector3 destination)
	{
		this.destination = destination;
		MoveTransition();
	}

	public void Stop()
	{
		IdleTransition();
	}

	public void Build(Building workplace)
	{
		this.workplace = workplace;
		BuildTransition();
	}

	public override void OnSelected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
			Selection.instance.moveEvent += Move;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
			Selection.instance.moveEvent -= Move;
		}
	}
}
