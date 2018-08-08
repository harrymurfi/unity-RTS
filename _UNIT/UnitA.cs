using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitA : Entitas
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
	public UnitState currentState;

	Vector3 destination;
	public Entitas targetEntity;

	//bool seeTarget;
	bool targetInRange;
	bool isAggresive;
	bool isOnHold;

	static float sphereCheckDelay = 0.5f;
	float nextAttack;
	float lastAttack;
	bool nextHit;
	static float attackRange = 1f;
	static float attackDelay = 1f;
	static float attackCastTime = 0.3f;
	static float visibilityRange = 10f;
	Collider[] enemyCollider = new Collider[1];

	public NavMeshPathStatus status;

	protected override void Start()
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
		status = agent.pathStatus;
//		switch(currentState)
//		{
//		case UnitState.Standby: StandbyState(); break;
//		case UnitState.Move: MoveState(); break;
//		case UnitState.Chase: ChaseState(); break;
//		case UnitState.Attack: AttackState(); break;
//		case UnitState.Dead: DeadState(); break;
//		}

		switch(currentState)
		{
		case UnitState.Standby: StandbyState(); break;
		case UnitState.Hold: HoldState(); break;
		case UnitState.Move: MoveState(); break;
		case UnitState.Chase: ChaseState(); break;
		case UnitState.Attack: AttackState(); break;
		case UnitState.Dead: DeadState(); break;
		case UnitState.None: break;
		}
	}

	protected virtual void StandbyTransition()
	{
		agent.enabled = false;
		obstacle.enabled = true;
		anim.SetBool(attackHash,false);
		anim.SetBool(runHash, false);
		StopAllCoroutines();
		StartCoroutine("SphereCheck");
		currentState = UnitState.Standby;
	}

	protected virtual void StandbyState()
	{
		if(targetEntity) 
		{
			if((targetEntity.transform.localPosition - transform.localPosition).sqrMagnitude < visibilityRange * visibilityRange)
			{
				ChaseTransition();
				return;
			}
			else
			{
				ClearTarget();
			}
		}
	}

	protected virtual void MoveTransition()
	{
		anim.SetBool(attackHash, false);
		anim.SetBool(runHash, true);
		StopAllCoroutines();
		StartCoroutine(Moving(destination, 1f));
		if(isAggresive) StartCoroutine("SphereCheck");
		currentState = UnitState.Move;
	}

	protected virtual void MoveState()
	{
		if(agent.enabled)
		{
			if(!agent.pathPending)
			{
				if(isAggresive)
				{
					if(targetEntity) ChaseTransition();
				}

				if(agent.remainingDistance <= agent.stoppingDistance)
				{
					StandbyTransition();
				}
			}
		}

	}

	protected virtual void ChaseTransition()
	{
		anim.SetBool(attackHash, false);
		StopAllCoroutines();
		StartCoroutine(Moving(targetEntity.transform.localPosition, attackRange));
		if(agent.pathStatus == NavMeshPathStatus.PathPartial)
		{
			StandbyTransition();
			return;
		}

		anim.SetBool(runHash, true);
		currentState = UnitState.Chase;
	}

	protected virtual void ChaseState()
	{
		if(targetEntity)
		{
			if(agent.enabled)
			{
				if(!agent.pathPending)
				{
					if(agent.remainingDistance <= agent.stoppingDistance)
					{
						if(agent.pathStatus == NavMeshPathStatus.PathPartial)
							StandbyTransition();
						else
							AttackTransition();
					}
					else
					{
						agent.SetDestination(targetEntity.transform.localPosition);
					}
				}
			}
		}
		else
		{
			StandbyTransition();
		}
	}

	protected virtual void AttackTransition()
	{
		agent.enabled = false;
		obstacle.enabled = true;
		nextHit = false;
		anim.SetBool(runHash, false);
		currentState = UnitState.Attack;
	}

	protected virtual void AttackState()
	{
		if(targetEntity)
		{
			if((targetEntity.transform.position - transform.position).sqrMagnitude > attackRange * attackRange + 2f)
			{
				ChaseTransition();
			}
			else
			{
				RotateToTarget(targetEntity.transform);
				if(Time.time > nextAttack)
				{
					lastAttack = Time.time;
					nextAttack = Time.time + attackDelay;
					nextHit = true;
					anim.SetBool(attackHash, true);
					//AudioController.instance.Play(0);
				}
				if(Time.time > lastAttack + attackCastTime && nextHit)
				{
					// play attack sound
					Hit();
					nextHit = false;
				}
				if(Time.time > lastAttack + 0.75f)
				{
					anim.SetBool(attackHash, false);
				}

			}
		}
		else
		{
			StandbyTransition();
		}
	}

	protected virtual void HoldTransition()
	{
		agent.enabled = false;
		obstacle.enabled = true;
		isAggresive = false;
		isOnHold = true;
		StopAllCoroutines();
		StartCoroutine("SphereCheck");
		anim.SetBool(attackHash,false);
		anim.SetBool(runHash, false);
		currentState = UnitState.Hold;
	}

	protected virtual void HoldState()
	{
		if(targetEntity)
		{
			RotateToTarget(targetEntity.transform);
			if((targetEntity.transform.localPosition - transform.position).sqrMagnitude < attackRange * attackRange + (attackRange/2f))
			{
				AttackTransition();
			}
		}
	}

	protected virtual void DeadTransition()
	{
		StopAllCoroutines();
		gameObject.layer = 0;
		obstacle.enabled = false;
		agent.enabled = false;
		anim.SetTrigger(deadHash);
		Selection.instance.moveEvent -= Move;
		Selection.instance.aggresiveMoveEvent -= AggresiveMove;
		Selection.instance.attackEvent -= Attack;
		Selection.instance.unitHoldEvent -= Hold;
		Selection.instance.unitStopEvent -= Stop;
		Destroy(hpBar.gameObject);
		Die();
		currentState = UnitState.Dead;
	}

	protected virtual void DeadState()
	{

	}

	protected virtual IEnumerator SphereCheck()
	{
		int range = 0;
		while(targetEntity == null)
		{
			if(Physics.OverlapSphereNonAlloc(transform.localPosition, visibilityRange, enemyCollider, player.enemyMask) > 0)
			{
				targetEntity = enemyCollider[0].GetComponent<Entitas>();
				targetEntity.deathEvent += OnEnemyDeath;
			}
			else
			{
				if(range < visibilityRange)
				{
					range += 2;
					//print("enemy not found 0.1s");
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				else
				{
					//print("enemy not found 0.5s");
					yield return new WaitForSeconds(sphereCheckDelay);
				}
			}
		}
		//print("Enemy found.");
	}

	IEnumerator Moving(Vector3 dest, float stoppingDist)
	{
		if(obstacle.enabled)
		{
			obstacle.enabled = false;
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		agent.enabled = true;
		agent.stoppingDistance = stoppingDist;
		agent.SetDestination(destination);
	}

	protected virtual void Hit()
	{
		targetEntity.GetComponent<Entitas>().TakeDamage(100f);
	}

	void ClearTarget()
	{
		targetEntity.deathEvent -= OnEnemyDeath;
		targetEntity = null;
	}

	public void OnEnemyDeath()
	{
		targetEntity.deathEvent -= OnEnemyDeath;
		targetEntity = null;
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
		isAggresive = false;
		this.destination = destination;
		MoveTransition();
	}

	public void AggresiveMove(Vector3 destination)
	{
		isAggresive = true;
		this.destination = destination;
		MoveTransition();
	}

	public void Attack(Entitas target)
	{
		isAggresive = false;
		this.targetEntity = target;
		AttackTransition();
	}

	public void Stop()
	{
		StandbyTransition();
	}

	public void Hold()
	{
		HoldTransition();
	}

	public override void OnSelected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
			Selection.instance.moveEvent += Move;
			Selection.instance.aggresiveMoveEvent += AggresiveMove;
			Selection.instance.attackEvent += Attack;
			Selection.instance.unitHoldEvent += Hold;
			Selection.instance.unitStopEvent += Stop;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != UnitState.Dead)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
			Selection.instance.moveEvent -= Move;
			Selection.instance.aggresiveMoveEvent -= AggresiveMove;
			Selection.instance.attackEvent -= Attack;
			Selection.instance.unitHoldEvent -= Hold;
			Selection.instance.unitStopEvent -= Stop;
		}
	}
}
