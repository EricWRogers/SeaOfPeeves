using System;
using System.Collections;
using System.Collections.Generic;
using SuperPupSystems.Helper;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
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
        public bool isClimbing = false;

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
            this.isClimbing = false;
        }
    }

    public abstract class MonsterAgent : MonoBehaviour
    {
        public AgentData AgentData;

        public AgentCombatState combatStates;

        //  private List<GameObject> attackers; //all agents about to attack
        public AudioSource swishAudio;
        private float attackCooldown = 0.0f;
        public GameObject playerGameObject;
        public float agentHealth;
        private int simultaneousAttackers = 1;
        public AgentCombatManager agentCombatManager;
        public PlayerAICombatState playerAICombatState;
        public GameObject attackIndicator;
        public GameObject damageCollider;
        public Animator monsterAnimator;
        private float randomizedSpeed;
        public NavMeshAgent navMeshAgent;
        public SplineAnimate splineAnimation;
        private void Start()
        {
            if(playerGameObject != null)
              playerAICombatState = playerGameObject.GetComponent<PlayerAICombatState>();
            agentCombatManager = new AgentCombatManager(this);
            agentHealth = AgentData.AgentHealth;
            randomizedSpeed = AgentData.movementSpeed + Random.Range(-.2f, .5f);

            if (AgentData.merlockType == MerlockType.Ship)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            
        }

        private void Update()
        {
            if (combatStates.isClimbing)
            {
                if (!splineAnimation.IsPlaying)
                {
                   monsterAnimator.SetBool("Climbing", false);
                   combatStates.isClimbing = false;
                }
            }
            else
            {
                agentCombatManager.UpdateAgentCombat();
            }
        }



        public bool OnAttack()
        {
            // Just to see if the monster is in a disabled state or not
            if (combatStates.stunned || combatStates.staggered || attackCooldown > 0.0f)
                return false;

            if(swishAudio != null)
            {
                swishAudio.Play();
            }

            return true;
        }

        public void OnRequestAttack(GameObject requestor)
        {
            // playerAICombatState.attackers.RemoveAll(item => item == null);

            if (playerAICombatState.attackers.Count < playerAICombatState.simultaneousAttackers)
            {
                if (!playerAICombatState.attackers.Contains(requestor))
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

        public void OnHit()
        {
            monsterAnimator.SetTrigger("Hit");
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
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, AgentData.trackSpeed * Time.deltaTime);
        }  
        public void TrackTargetAttack()
        {
            Vector3 targetPoint = playerGameObject.transform.position;
            targetPoint.y = transform.position.y;

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }

        public void MoveToTarget(Vector3 targetPosition)
        {
            StopCoroutine();
            Vector3 direction = (targetPosition - transform.position).normalized;
            TrackTarget(playerGameObject);

            if (AgentData.merlockType == MerlockType.UnderWater)
            {
                Vector3 moveVector = direction * (randomizedSpeed * Time.deltaTime);

                // Ensure only horizontal movement by maintaining the current y position
                moveVector.y = 0;

                transform.position += moveVector;
            }
            else
            {
                if(navMeshAgent.isOnNavMesh)
                 navMeshAgent.destination = targetPosition;
                navMeshAgent.speed = AgentData.movementSpeed;
            }
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            monsterAnimator.SetFloat("MoveY", localDirection.z);
            monsterAnimator.SetFloat("MoveX", localDirection.x);
        }

        public void ResetAnimations()
        {
            StartCoroutine(SmoothToIdle());
        }

        public void StopCoroutine()
        {
            StopCoroutine(SmoothToIdle());
        }

        public void StartDeathAnimations()
        {
            monsterAnimator.SetTrigger("Die");
            combatStates.stunned = true;
            if (AgentData.merlockType == MerlockType.Ship)
                navMeshAgent.destination = transform.position;
            StartCoroutine(Death());
        }

        private IEnumerator SmoothToIdle()
        {
            float y = monsterAnimator.GetFloat("MoveY");
            float x = monsterAnimator.GetFloat("MoveX");
            float smoothTime = 1.0f; // Time to smooth back to idle

            while (Mathf.Abs(y) > 0.01f || Mathf.Abs(x) > 0.01f)
            {
                // Smoothly damp the values towards zero
                y = Mathf.MoveTowards(y, 0, Time.deltaTime / smoothTime);
                x = Mathf.MoveTowards(x, 0, Time.deltaTime / smoothTime);

                monsterAnimator.SetFloat("MoveY", y);
                monsterAnimator.SetFloat("MoveX", x);

                yield return null; // Wait for the next frame
            }

            // Ensure final values are set to zero
            monsterAnimator.SetFloat("MoveY", 0);
            monsterAnimator.SetFloat("MoveX", 0);
        }
        
        private IEnumerator Death()
        {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }
    }
}