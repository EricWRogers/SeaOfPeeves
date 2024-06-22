using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [System.Serializable]
    public class AgentCombatState
    {
        public bool dodging = false;
        public bool dodgingRecovery = false;
        public bool blocking = false;
        public bool attacking = false;
        public bool Lunging = false;
        public bool attackingRecovery = false;
        public bool stunned = false;
        public bool staggered = false;
        public bool running = false;
        public bool unbalanced = false;
        public bool strafing = false;
        public bool avoiding = false;
        public bool isGettingHit = false;
        public bool IsAttacking = false;

        public void Reset()
        {
            this.dodging = false;
            this.dodgingRecovery = false;
            this.blocking = false;
            this.attacking = false;
            this.Lunging = false;
            this.attackingRecovery = false;
            this.stunned = false;
            this.staggered = false;
            this.running = false;
            this.unbalanced = false;
        }
    }
    public abstract class MonsterAgent : MonoBehaviour
    {
        public AgentData AgentData;
        public AgentCombatState combatStates;
        private List<GameObject> attackers; //all agents about to attack
        private float attackCooldown = 0.0f;
        public GameObject playerGameObject;
        public float agentHealth;
        private int simultaneousAttackers = 1;
        public AgentCombatManager agentCombatManager;
        public GameObject attackIndicator;
        private void Start()
        {
            attackers = new List<GameObject>();
            agentCombatManager = new AgentCombatManager(this);
            agentHealth = AgentData.AgentHealth;
        }

        private void Update()
        {
            agentCombatManager.UpdateAgentCombat();
        }

        public void LookAtTarget(GameObject target)
        {
            transform.LookAt(target.transform);
        }

        public void TrackTarget(GameObject target)
        {
	        var targetPoint = target.transform.position;
	        targetPoint.y = transform.position.y;
            
	        var targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
	        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AgentData.trackSpeed);
        }
        
        public bool OnAttack()
        {
            // Just to see if the monster is in a disabled state or not
            if ( combatStates.stunned || combatStates.staggered || attackCooldown > 0.0f)
                return false;
            
            return true;
        }
        
        public void OnRequestAttack(GameObject requestor)
        {
            attackers.RemoveAll(item => item == null);

            if (attackers.Count < simultaneousAttackers)
            {
                if (!attackers.Contains(requestor))
                    attackers.Add(requestor);
                //Debug.Log("Requestor " + requestor);
                
                agentCombatManager.OnAllowAttack(requestor);
                // Debug.Log("Attack accepted, current attackers: " + attackers.Count);
            }
            else
            {
                // Debug.Log("Attack REJECTED, current attackers: " + attackers.Count);
            }
        }
        
        public void OnCancelAttack(GameObject requestor)
        {
            // Debug.Log("Requestor " + requestor);
            attackers.Remove(requestor);
        }
        
        public void MoveToTarget( Vector3 targetPosition )
        {
            // Check if close enough to target
            if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, AgentData.movementSpeed * Time.deltaTime);
            }
        }
    }
}