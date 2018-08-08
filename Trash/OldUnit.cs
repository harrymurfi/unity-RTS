using UnityEngine;
using System.Collections;

public class OldUnit : Entity {

	public UnitType type;

	NavMeshAgent agent;

	public UnitState curState;
	public float lineOfSight;
	public LayerMask enemyMask;

	Transform thisTransform;
	public Transform thisTarget;
	public Vector3 thisDestination;
	public float attackRange;
	public float attackTime;

	float nextScan, nextAttack;
	bool hasTarget;
	bool targetInRange;
	Vector3 patrolA, patrolB;

	// Track animation
	Renderer trackRend;
	float offset;
	float offsetCount;
	float vel;

	protected override void Start ()
	{
		base.Start ();

		thisTransform = GetComponent<Transform>();
		agent = GetComponent<NavMeshAgent>();
		curState = UnitState.Stop;
		trackRend = transform.Find("Model/Track").gameObject.GetComponent<Renderer>();
	}

	protected override void Update ()
	{
		base.Update ();

		if(thisTarget != null)
			hasTarget = true;
		else
			hasTarget = false;

		switch(curState)
		{
		case UnitState.Move: MoveNode(); break;
		case UnitState.Stop: StopNode(); break;
		case UnitState.Attack: AttackNode(); break;
		case UnitState.Patrol: PatrolNode(); break;
		case UnitState.Hold : HoldNode(); break;
		}
			
	}

	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
	}


	// State Node
	public void ToMoveTransition(Vector3 destination)
	{
		thisTarget = null;

		if(destination != thisDestination && destination != Vector3.zero)
		{
			thisDestination = destination;
			curState = UnitState.Move;
		}
		else
		{
			return;
		}
	}

	void MoveNode()
	{
		agent.SetDestination(thisDestination);
		TrackAnim();
		if((thisDestination - thisTransform.position).sqrMagnitude < Mathf.Pow(agent.stoppingDistance, 2))
		{
			ToStopTransition();
		}
	}

	public void ToStopTransition()
	{
		thisTarget = null;
		thisDestination = Vector3.zero;
		curState = UnitState.Stop;
	}

	void StopNode()
	{
		if(!hasTarget)
		{
			ScanForTarget();
		}	
		else
		{
			ToAttackTransition(thisTarget);
		}
	}

	public void ToAttackTransition(Transform target)
	{
		thisTarget = target;
		thisDestination = Vector3.zero;
		curState = UnitState.Attack;
	}

	void AttackNode()
	{
		if(hasTarget)
		{
			if(TargetInRange(thisTarget))
			{
				AttackTarget();
			}
			else
			{
				agent.SetDestination(thisTarget.position);
				TrackAnim();
			}
		}
		else
		{
			ToStopTransition();
		}
	}

	public void ToPatrolTransition(Vector3 destination)
	{
		thisTarget = null;
		thisDestination = Vector3.zero;
		patrolA = thisTransform.position;
		patrolB = destination;
		curState = UnitState.Patrol;
	}

	void PatrolNode()
	{
		if(!hasTarget)
		{
			ScanForTarget();
		}
		else
		{
			ToAttackTransition(thisTarget);
		}
	}

	public void ToHoldTransition()
	{
		thisTarget = null;
		thisDestination = Vector3.zero;
		curState = UnitState.Hold;
	}

	void HoldNode()
	{
		if(!hasTarget)
		{
			ScanForTarget();
		}
		else
		{
			if(TargetInRange(thisTarget))
			{
				AttackTarget();
			}
			else
			{
				thisTarget = null;
			}
		}
	}

	// Behaviour
	void ScanForTarget()
	{
		if(Time.time > nextScan)
		{
			Collider[] enemies = Physics.OverlapSphere(thisTransform.position, lineOfSight, enemyMask, QueryTriggerInteraction.Ignore);
			if(enemies.Length > 0)
			{
				thisTarget = enemies[0].transform;
			}
			
			nextScan = Time.time + 0.5f;
		}
	}

	void AttackTarget()
	{
		if(Time.time > nextAttack)
		{

			nextAttack = Time.time + attackTime;
		}
	}

	bool TargetInRange(Transform target)
	{
		if(hasTarget)
		{
			if((target.position - thisTransform.position).sqrMagnitude <= attackRange * attackRange)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	void TrackAnim()
	{
		vel = agent.velocity.sqrMagnitude;
		if(vel > 4)
		{
			trackRend.material.SetTextureOffset("_MainTex", new Vector2(0, -offset));
			offset = Mathf.Repeat(offsetCount, 10f); 
			offsetCount += 0.2f * Time.deltaTime;
		}
	}
}
