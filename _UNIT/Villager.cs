using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Villager : Entitas
{
	static int runHash = Animator.StringToHash("Run");
	static int workHash = Animator.StringToHash("Work");
	static int deadHash = Animator.StringToHash("Dead");

	Animator anim;
	NavMeshAgent agent;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	Image hpbarImage;
	HPBar hpBar;
	public UnitState currentState = UnitState.Idle;

	Vector3 destination;
	Entitas targetEntity;
	Building workplace;

	protected override void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		anim = GetComponentInChildren<Animator>();
		agent= GetComponent<NavMeshAgent>();
		agent.enabled = false;
		obstacle = GetComponent<NavMeshObstacle>();

		hpBar = GetComponentInChildren<HPBar>();
		hpBar.anchor = transform;
		hpbarImage = hpBar.GetComponent<Image>();
		hpbarImage.enabled = false;
		hpBar.transform.SetParent(UIController.instance.attributesContainer.transform, false);

		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;
	}

	void Update()
	{
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
		ResetWorkplace();
		agent.enabled = false;
		obstacle.enabled = true;
		anim.SetBool(runHash, false);
		anim.SetBool(workHash, false);
		currentState = UnitState.Idle;
	}

	void IdleState()
	{
		
	}

	void MoveTransition()
	{
		ResetWorkplace();
		anim.SetBool(workHash, false);
		anim.SetBool(runHash, true);
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

	void BuildTransition(Building building)
	{
		this.workplace = building;
		this.workplace.OnBuildComplete += OnBuildingComplete;
		this.workplace.deathEvent += OnBuildingDestroyed;
		anim.SetBool(runHash, true);
		StopAllCoroutines();
		StartCoroutine(Moving(building.spawnPoint.transform.position, 1f));
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
					anim.SetBool(runHash, false);
					anim.SetBool(workHash, true);
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
		Selection.instance.onRightSelectFriendlyBuilding -= OnRightSelectFriendlyBuilding;
		ResetWorkplace();
		Destroy(hpBar.gameObject);
		Die();
		currentState = UnitState.Dead;
	}

	void DeadState()
	{

	}

	IEnumerator Moving(Vector3 dest, float stoppingDist)
	{
		if(agent.enabled) 
		{
			agent.enabled = false;
			yield return new WaitForEndOfFrame();
		}
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

	void ResetWorkplace()
	{
		if(workplace != null)
		{
			workplace.OnBuildComplete -= OnBuildingComplete;
			workplace.deathEvent -= OnBuildingDestroyed;
			workplace.DeassignBuilder();
			workplace = null;
		}
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

	public void Build(Building workplace)
	{
		BuildTransition(workplace);
	}

	public void Stop()
	{
		IdleTransition();
	}

	public void OnRightSelectFriendlyBuilding(Building building)
	{
		ResetWorkplace();
		BuildTransition(building);
	}

	public override void OnSelected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
			Selection.instance.moveEvent += Move;
			Selection.instance.onRightSelectFriendlyBuilding += OnRightSelectFriendlyBuilding;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
			Selection.instance.moveEvent -= Move;
			Selection.instance.onRightSelectFriendlyBuilding -= OnRightSelectFriendlyBuilding;
		}
	}

	public void OnBuildingComplete()
	{
		IdleTransition();
	}

	public void OnBuildingDestroyed()
	{
		IdleTransition();
	}
}
