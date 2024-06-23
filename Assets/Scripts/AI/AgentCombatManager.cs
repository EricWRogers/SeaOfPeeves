using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class AgentCombatManager 
{
    
    private Vector3 destination;
    private Vector3 moveVec;
    public float attackDistance = 1.0f;
    public float dangerDistance = 2.0f;
    public float trackSpeed = 0.1f;
    public float attackRate = 10.0f;
    public float attackRateFluctuation = 0.0f;

    
    //For melee agents with range capabilities
    public float subAttackRate = 10.0f;
    public float subAttackRateFluctuation = 0.0f;
    private float lastAttackTime = 0.0f;
    private float lastSubAttackTime = 0.0f;

    private bool disabled = false;
    private float lastThought = 0.0f;
    private float lastReact = 0.0f;
    private float actualAttackRate = 0.0f;
    private float actualSubAttackRate = 0.0f;


    private float thinkPeriod = 1.5f;
    private float reactPeriod = 0.4f;
    public GameObject targetObject;

    private Vector3 distVec;
    private Vector3 avoidVec = Vector3.zero;
    private float sqrDistance;
    private float sqrAttackDistance;
    private float subWeaponAttackDistance;
    private float sqrDangerDistance;
    private bool engagePrey = false;
    bool strafeDirection = false;
    private float strafeCooldown = 0f;
    private float strafeRate = 3.0f;
    private float rangeTimeOut = 0f;
    private float avoiderTimeOut = 0f;

    private Avoider avoider;
    public MonsterAgent ThisAgent { get; set; }

 
	private System.Random random = new System.Random();
    float enemyMeleeDistance;
    public float EnemySqrMeleeRange => enemyMeleeDistance * enemyMeleeDistance;
    private float attackCooldown => Mathf.Max(actualAttackRate - (Time.fixedTime - lastAttackTime), 0f);

    private float subAttackCooldown => Mathf.Max(actualSubAttackRate - (Time.fixedTime - lastSubAttackTime), 0f);

    private float placeHolderAnimationStart;
    #region Combat Functions

    //Combat manager constructor
	public AgentCombatManager(MonsterAgent Agent)
	{
		ThisAgent = Agent;
		avoider = ThisAgent.GetComponentInChildren<Avoider>();

		attackDistance = ThisAgent.AgentData.attackDistance;
		dangerDistance = ThisAgent.AgentData.dangerDistance;
		attackRate = ThisAgent.AgentData.attackRate;
		
		trackSpeed = ThisAgent.AgentData.trackSpeed;
		attackRateFluctuation = ThisAgent.AgentData.attackRateFluctuation;
		actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;
		lastAttackTime = -actualAttackRate;

		subAttackRate = ThisAgent.AgentData.subAttackRate;
		subAttackRateFluctuation = ThisAgent.AgentData.subAttackRateFluctuation;
		actualSubAttackRate = subAttackRate + (Random.value - 0.5f) * subAttackRateFluctuation;
		lastSubAttackTime = -subAttackRate;
		
		if (avoider != null)
		{
			avoider.gameObject.GetComponent<SphereCollider>().radius = ThisAgent.AgentData.seperation;
		}

		sqrAttackDistance = Mathf.Pow(attackDistance, 2);
		sqrDangerDistance = Mathf.Pow(dangerDistance, 2);
		
		// offset the start of the think ticks to spread the load out a little
		lastThought += thinkPeriod * Random.value;
		lastReact += reactPeriod * Random.value;
		if (ThisAgent.AgentData.strafeRate != 0)
		{
			strafeRate = ThisAgent.AgentData.strafeRate;
		}
		
	}
	
    public void UpdateAgentCombat()
	{
		BattleCircleAI(); 
		//todo: Add any relavant combat changes we need - if in player attack range, shooting, carrying stuff etc  

		if (targetObject != null)
		{
			// //Attacking Fact -- should be true as merlock or sea monster attack anims are playing
			// if ()
			// {
			// 	ThisAgent.combatStates.attacking = true;
			// }
			// else
			// {
			// 	ThisAgent.combatStates.attacking = false;
			// }
			//
			// //tracking opponent on melee attack
			// if (ThisAgent.combatStates.IsAttacking)
			// {
			// 	ThisAgent.TrackTarget(targetObject);
			// }
		}
	}
	void UpdateMeleeCombat()
    {
	    
    }

    #endregion
    
    
    
    
    #region Battle Circle AI Logic

	void UpdateDistance()
	{
		distVec = destination - ThisAgent.transform.position;
		sqrDistance = distVec.sqrMagnitude;

		if (strafeCooldown > 0.0f)
		{
			strafeCooldown -= Time.fixedDeltaTime;
		}


		if (sqrDistance > sqrAttackDistance)
		{
			rangeTimeOut += Time.fixedDeltaTime;
		}

		if (rangeTimeOut > 0.0f && sqrDistance <= sqrAttackDistance)
		{
			rangeTimeOut -= Time.fixedDeltaTime;
		}


		//Avoider time out
		if (avoidVec != Vector3.zero && avoiderTimeOut <= 0 && sqrDistance <= sqrDangerDistance)
		{
			avoiderTimeOut += Time.fixedDeltaTime;
		}

		if (avoidVec == Vector3.zero && avoiderTimeOut > 0)
		{
			avoiderTimeOut -= Time.fixedDeltaTime;
		}


		//Throwing spears 
		if (subWeaponAttackDistance > sqrAttackDistance)
		{
			//  if (sqrDistance > subWeaponAttackDistance && engagePrey)
			//  {

			//      OnAttackComplete();


			//  }
		}
		else
		{
			//  if (sqrDistance > sqrAttackDistance && engagePrey )
			//  {

			//      OnAttackComplete();


			//  }
		}
	}

	void BattleCircleAI()
	{
		if (engagePrey && !ThisAgent.combatStates.attacking)
		{
			OnAttackComplete();
		}

		//Place holder 
		if (ThisAgent.combatStates.attacking)
		{
			Debug.Log($"Place holder time since animation start:  {Time.fixedTime - placeHolderAnimationStart}");
			if (Time.fixedTime - placeHolderAnimationStart > .5)
			{
				if (ThisAgent.damageCollider != null)
				{
					ThisAgent.damageCollider.SetActive(true);
				}
			}
			
			if (Time.fixedTime - placeHolderAnimationStart > 1.5)
			{
				ThisAgent.combatStates.attacking = false;
			}
		}

		// keep looking at the target even if it's disabled
		if (ThisAgent.combatStates.stunned)
		{
			if (targetObject != null)
			{
				lastReact = Time.fixedTime;
				UpdateDistance();
			}

			return;
		}
		
		// Update think period at fixed intervals 
		if (targetObject == null || (Time.fixedTime - lastThought) > thinkPeriod)
		{
			lastThought = Time.fixedTime;
			Think();
			
		}

		if (targetObject == null)
		{
			return;
		}

		if ((Time.fixedTime - lastReact) > reactPeriod)
		{
			React();
		}

		UpdateDistance();
		
		bool shouldAvoid = ( avoidVec != Vector3.zero && sqrDistance <= sqrDangerDistance &&
				ThisAgent.AgentData.AvoidOthers ||  avoiderTimeOut > 0f);
		bool shouldStrafe =
			((!ThisAgent.combatStates.staggered && !shouldAvoid && !engagePrey &&
			  sqrDistance <= sqrAttackDistance ||
			  !!ThisAgent.combatStates.staggered && !shouldAvoid && !engagePrey &&
			  sqrDistance > sqrAttackDistance && rangeTimeOut < 1));
		bool shouldAttack =
			((engagePrey && sqrDistance <= sqrAttackDistance || engagePrey && sqrDistance > sqrAttackDistance &&
				sqrDistance <= subWeaponAttackDistance));
		
		ThisAgent.combatStates.strafing = shouldStrafe;
		ThisAgent.combatStates.avoiding = shouldAvoid;


		if (shouldAvoid)
		{
			Avoid(avoidVec);
		}
		else if (shouldAttack)
		{
			// I have permission for regular attack
			if (sqrDistance <= sqrAttackDistance)
			{
				Attack(targetObject.transform.position);
			}

			if (sqrDistance > sqrAttackDistance && sqrDistance <= subWeaponAttackDistance ||
			    subWeaponAttackDistance < sqrAttackDistance && sqrDistance <= subWeaponAttackDistance)
			{
				// Todo: if we have spears 
				//SubAttack(targetObject.transform.position);
			}
		}
		else if (shouldStrafe)
		{
			Strafe();
		}

		else
		{
			if (!ThisAgent.combatStates.strafing && !ThisAgent.combatStates.avoiding)
			{

				if (sqrDistance > sqrAttackDistance)
				{
					Seek(targetObject.transform.position);
				}
			}
		}
		
	}

	void Seek(Vector3 distVec)
	{
		if (ThisAgent.AgentData.canLockOn == true)
		{
			Seek(distVec, true);
		}
		else
		{
			Seek(distVec, false);
		}
	}

	void Seek(Vector3 distVec, bool align)
	{
		// whenever I decide to move, I am giving up permission to attack
		if (engagePrey)
		{
			OnAttackComplete();
		}


		if (align == true)
		{
			if (sqrDistance <= sqrDangerDistance)
			{
				//ThisAgent.LookAtTarget(targetObject);
				ThisAgent.TrackTarget(targetObject);
				
			}
			else
			{
			
				//if (ThisAgent.combatStates.strafing || ThisAgent.combatStates.avoiding )
				//{
					ThisAgent.TrackTarget(targetObject);
					
				//}
			}
		}
		
		//check to see if withing my fov
		destination = targetObject.transform.position;
		moveVec = distVec;
		ThisAgent.MoveToTarget(moveVec);
	}


	//Passing in the vector of what you need to avoid
	void Avoid(Vector3 distVec)
	{
		if (avoidVec != Vector3.zero && avoider.avoidEnemy != null)
		{
			Vector3 forward = ThisAgent.transform.forward;
			Vector3 toOther = (avoider.avoidEnemy.transform.position - ThisAgent.transform.position).normalized;
			Vector3 avoidancePosition = Vector3.zero;

			// Check if the enemy is in front
			if (Vector3.Dot(forward, toOther) > 0.7f)
			{
				// Enemy is in front, strafe left or right
				Vector3 localPos = ThisAgent.transform.InverseTransformPoint(avoider.avoidEnemy.transform.position);
				bool right = localPos.x >= 0.0f;

				if (right)
				{
					avoidancePosition = ThisAgent.transform.position + -ThisAgent.transform.right * ThisAgent.AgentData.seperation / 2;
					Debug.DrawLine(ThisAgent.transform.position,avoidancePosition,Color.green );
				}
				else
				{
					avoidancePosition = ThisAgent.transform.position + ThisAgent.transform.right * ThisAgent.AgentData.seperation / 2;
					Debug.DrawLine(ThisAgent.transform.position,avoidancePosition,Color.cyan );
				}
			}
			else
			{
				// Enemy is not in front, avoid normally
				Vector3 localPos = ThisAgent.transform.InverseTransformPoint(avoider.avoidEnemy.transform.position);
				bool right = localPos.x < 0.0f;

				if (!right)
				{
					avoidancePosition = ThisAgent.transform.position + -avoidVec * ThisAgent.AgentData.seperation;
					Debug.DrawLine(ThisAgent.transform.position,avoidancePosition,Color.yellow );

				}
				else
				{
					avoidancePosition = ThisAgent.transform.position + -avoidVec * ThisAgent.AgentData.seperation;
					Debug.DrawLine(ThisAgent.transform.position,avoidancePosition,Color.blue );
				}
			}

			Seek(avoidancePosition, ThisAgent.AgentData.canLockOn);
		}
	}

	void Strafe()
	{
		//Debug.Log("Trying to strafe");
		Vector3 strafePosition = Vector3.zero;

		if (!engagePrey)
		{
			if (!strafeDirection)
			{
				strafePosition = targetObject.transform.position + ThisAgent.transform.right * attackDistance;
			}
			else
			{
				strafePosition = targetObject.transform.position + -ThisAgent.transform.right * attackDistance;
			}
		}

		if (ThisAgent.AgentData.canLockOn)
		{
			Seek(strafePosition, true);
		}
		else
		{
			Seek(strafePosition, false);
		}
	}

	
	IEnumerator OnWait(float delay)
	{
		this.Disable();
		yield return new WaitForSeconds(delay);
		this.Enable();
	}

	void Attack(Vector3 target)
	{
		if ( !ThisAgent.combatStates.attacking && attackCooldown <= 0.0f)
		{
			bool success = false;

			success = ThisAgent.OnAttack();

			if (success)
			{
				ThisAgent.combatStates.attacking = true;
				
				Debug.Log("On Success Attack");
				if (ThisAgent.attackIndicator != null)
				{
					ThisAgent.attackIndicator.SetActive(true);
				}
				
				placeHolderAnimationStart = Time.fixedTime;
				lastAttackTime = Time.fixedTime;
				actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;
			    //Debug.Log("Attack Flux" + actualAttackRate);
			}
		}
	}

	void SubAttack(Vector3 target)
	{
		if (!ThisAgent.combatStates.attacking && subAttackCooldown <= 0.0f)
		{
			bool success = false;

			success = ThisAgent.OnAttack();

			if (success)
			{
				//Debug.Log("Attack allowed");
				if (ThisAgent.attackIndicator != null)
				{
				    ThisAgent.attackIndicator.SetActive(true);
				}
				
				// Debug.Log("Successful sub attack request");
				lastSubAttackTime = Time.fixedTime;
				actualSubAttackRate = subAttackRate + (Random.value - 0.5f) * subAttackRateFluctuation;
			}
		
		}
	}

	//Todo: Add lunge attacks if time permits
	
	GameObject GetClosestTarget()
	{
		//todo: Can expand on this if we want monsters to target other things besides the player ie ship 
		return ThisAgent.playerGameObject;
	}


	//Sets up if there is a vector to avoid or if the strafe cooldown has elasped
	void Think()
	{
		targetObject = GetClosestTarget();
		
		// nothing to kill!
		if (targetObject == null)
			return;
		
		// for avoiding the other merlocks
		if (avoider != null && avoider.avoidEnemy != null)
		{
			MonsterAgent checkAgent = avoider.avoidEnemy.gameObject.GetComponent<MonsterAgent>();
			Vector3 CheckVector = avoider.avoidEnemy.transform.position - ThisAgent.transform.position;

			//Dont worry about avoiding dead merlocks 
			if (checkAgent.agentHealth <= 0 )
			{
				avoider.avoidEnemy = null;
				avoidVec = Vector3.zero;
			}
			else
			{
				avoidVec = CheckVector;
				avoidVec.Normalize();
			}
		}

		else
		{
			avoidVec = Vector3.zero;
		}

		// for strafing, if merlock decides to strafe
		if (!engagePrey && strafeCooldown <= 0f && avoidVec == Vector3.zero)
		{
			strafeCooldown = strafeRate;
		}
	}

	//lets the agent request an attack if the period to react has elasped 
	void React()
	{
		lastReact = Time.fixedTime;
		
		UpdateMeleeCombat();
		
		sqrDistance = Vector3.SqrMagnitude(targetObject.transform.position - ThisAgent.transform.position);

		//Main Weapon on Request Attack
		if (sqrDistance != 0 && sqrDistance <= sqrAttackDistance)
		{
			if (!engagePrey)
			{
				ThisAgent.OnRequestAttack(ThisAgent.gameObject);
			}
		}
		
		if (sqrDistance != 0 && sqrDistance > sqrAttackDistance && sqrDistance <= subWeaponAttackDistance)
		{
			if (!engagePrey)
			{
				ThisAgent.OnRequestAttack(ThisAgent.gameObject);
			}
		}
		
	}

	public void ResetLastAttackTime()
	{
		lastAttackTime = 0.0f;
	}

	public void ResetLastStrafeTime()
	{
		strafeCooldown = 0.0f;
	}

	public void OnAllowAttack(GameObject target)
	{
		if (targetObject != null && target == ThisAgent.gameObject)
			engagePrey = true;
		//   Debug.Log("Target " + target + "preyObject " + preyObject);
	}

	void OnAttackComplete()
	{
		// disengage when completing an attack to give other enemies a chance
		// Todo: this currently only happens if not in range of my prey need to set it for when attack chain or animation is done 
		Debug.Log("Attack Done ");
		engagePrey = false;
		if (ThisAgent.attackIndicator != null)
		{
			ThisAgent.attackIndicator.SetActive(false);
		}
		if (ThisAgent.damageCollider != null)
		{
			ThisAgent.damageCollider.SetActive(false);
		}
		
		if (targetObject != null)
		{
			ThisAgent.OnCancelAttack(ThisAgent.gameObject);
		}
		
	}

	void OnStun(float d)
	{
		OnAttackComplete();
	}

	public void OnDeath()
	{
		OnAttackComplete();
	}


	void Enable()
	{
		disabled = false;
	}

	void Disable()
	{
		disabled = true;
	}

	#endregion
}
