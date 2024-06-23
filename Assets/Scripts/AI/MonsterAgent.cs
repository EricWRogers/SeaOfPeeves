using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
      //  private List<GameObject> attackers; //all agents about to attack
        private float attackCooldown = 0.0f;
        public GameObject playerGameObject;
        public float agentHealth;
        private int simultaneousAttackers = 1;
        public AgentCombatManager agentCombatManager;
        private PlayerAICombatState playerAICombatState;
        public GameObject attackIndicator;
        public GameObject damageCollider;

        private float randomizedSpeed;
        private void Start()
        {
            playerAICombatState = playerGameObject.GetComponent<PlayerAICombatState>();
            agentCombatManager = new AgentCombatManager(this);
            agentHealth = AgentData.AgentHealth;

            randomizedSpeed = AgentData.movementSpeed + Random.Range(-.2f, .5f);
        }

        private void Update()
        {
            agentCombatManager.UpdateAgentCombat();
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
           // playerAICombatState.attackers.RemoveAll(item => item == null);

            if ( playerAICombatState.attackers.Count < playerAICombatState.simultaneousAttackers)
            {
                if (! playerAICombatState.attackers.Contains(requestor))
                    playerAICombatState.attackers.Add(requestor);
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
            if (attackIndicator != null)
            {
               attackIndicator.SetActive(false);
            }
            if (damageCollider != null)
            {
                damageCollider.SetActive(false);
            }
            // Debug.Log("Requestor " + requestor);
            playerAICombatState.attackers.Remove(requestor);
        }
   
        public void TrackTarget(GameObject target)
        {
            Vector3 targetPoint = target.transform.position;
            targetPoint.y = transform.position.y; 

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AgentData.trackSpeed * Time.deltaTime);
        }
        public void MoveToTarget( Vector3 targetPosition )
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 moveVector = direction * (randomizedSpeed * Time.deltaTime);
        
            // Ensure only horizontal movement by maintaining the current y position
            moveVector.y = 0;

            transform.position += moveVector;
        }
    }
}