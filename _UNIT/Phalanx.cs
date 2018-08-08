using UnityEngine;
using UnityEngine.UI;

public class Phalanx : Entitas
{
	static int runHash = Animator.StringToHash("Run");
	static int attackHash = Animator.StringToHash("Attack");
	static int deadHash = Animator.StringToHash("Dead");

	Animator anim;
	NavMeshAgent agent;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	public Image hpbarImage;
	public HPBar hpBar;
	public UnitState currentState = UnitState.Idle;

	Vector3 destination;
	Entitas targetEntity;

	bool seeTarget;
	bool targetInRange;

	public LayerMask enemyMask;
	static float sonarCheckTime = 0.5f;
	float lastSonarCheck;
	float nextAttack;
	float lastAttack;
	bool nextHit;
	static float attackRange = 2f;
	static float attackDelay = 1f;
	static float attackCastTime = 0.3f;
	static float visibilityRange = 10f;
	Collider[] enemyCollider = new Collider[1];

	protected override void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		anim = GetComponentInChildren<Animator>();
		agent= GetComponent<NavMeshAgent>();
		obstacle = GetComponent<NavMeshObstacle>();
		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;
		hpbarImage.enabled = false;
		hpBar = hpbarImage.GetComponent<HPBar>();
	}

	void Update()
	{
		switch(currentState)
		{
		case UnitState.Idle: IdleState(); break;
		case UnitState.Move: MoveState(); break;
		case UnitState.Chase: ChaseState(); break;
		case UnitState.Attack: AttackState(); break;
		case UnitState.Dead: DeadState(); break;
		}
	}

	void IdleTransition()
	{
		agent.avoidancePriority = 10;
		anim.SetBool(attackHash,false);
		anim.SetBool(runHash, false);
		currentState = UnitState.Idle;
	}

	void IdleState()
	{
		if(targetEntity != null) 
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
		Sonar();
	}

	void MoveTransition()
	{
		agent.avoidancePriority = 90;
		anim.SetBool(attackHash, false);
		anim.SetBool(runHash, true);
		agent.stoppingDistance = 1;
		agent.SetDestination(destination);
		currentState = UnitState.Move;
	}

	void MoveState()
	{
		if(!agent.pathPending)
		{
			if(agent.remainingDistance <= agent.stoppingDistance)
			{
				IdleTransition();
			}
		}
	}

	void ChaseTransition()
	{
		agent.avoidancePriority = 90;
		anim.SetBool(attackHash, false);
		agent.SetDestination(targetEntity.transform.localPosition);
		if(agent.pathStatus == NavMeshPathStatus.PathPartial)
		{
			IdleTransition();
			return;
		}

		anim.SetBool(runHash, true);
		agent.stoppingDistance = attackRange;
		currentState = UnitState.Chase;
	}

	void ChaseState()
	{
		if(targetEntity)
		{
			if(!agent.pathPending)
			{
				if(agent.remainingDistance <= agent.stoppingDistance)
				{
					if(agent.pathStatus == NavMeshPathStatus.PathPartial)
						IdleTransition();
					else
						AttackTransition();
				}
				else
				{
					agent.SetDestination(targetEntity.transform.localPosition);
				}
			}
		}
		else
		{
			IdleTransition();
		}
	}

	void AttackTransition()
	{
		agent.avoidancePriority = 10;
		nextHit = false;
		anim.SetBool(runHash, false);
		currentState = UnitState.Attack;
	}

	void AttackState()
	{
		if(targetEntity)
		{
			if((targetEntity.transform.localPosition - transform.position).sqrMagnitude > 6f)
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

			IdleTransition();
		}
	}

	void DeadTransition()
	{
		gameObject.layer = 0;
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

	void Sonar()
	{
		if(Time.time > lastSonarCheck)
		{
			lastSonarCheck = Time.time + sonarCheckTime;
			if(Physics.OverlapSphereNonAlloc(transform.localPosition, visibilityRange, enemyCollider, enemyMask) > 0)
			{
				targetEntity = enemyCollider[0].GetComponent<Entitas>();
				targetEntity.deathEvent += OnEnemyDeath;
			}
		}
	}

	void Hit()
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
		this.destination = destination;
		MoveTransition();
	}

	public void Attack(Entitas target)
	{
		this.targetEntity = target;
		AttackTransition();
	}

	public void Stop()
	{
		IdleTransition();
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
