using UnityEngine;

namespace DefaultNamespace
{
   public enum MerlockType
    {
        UnderWater,
        Ship
    }
    [CreateAssetMenu(fileName = "AgentData", menuName = "Agent Data")]
    public class AgentData : ScriptableObject
    {
        [Header(" Agent Combat Settings")]
        public MerlockType merlockType = MerlockType.UnderWater;
        public float AgentHealth = 1.0f;
        public float movementSpeed = 1.0f;
        public float attackDistance = 1.0f;
        public float dangerDistance = 2.0f;
        public float trackSpeed = 0.1f;
        public float attackRate = 10.0f;
        public float attackRateFluctuation = 0.0f;
        public float subAttackRate = 10.0f;
        public float subAttackRateFluctuation = 0.0f;
        public float seperation = 0f;
        public float strafeRate = 0f;
        public float AgentBaseDamage = 0f;
        public float AgentBaseDefense = 0f;
        public float AgentEvadeChance;
        public float AgentCounterChance;
        public float AgentSkillChance;
        public float AgentLungeChance;
        public bool CanDodge = false;
        public bool AvoidOthers = true;
        public bool canLockOn = false;
    }
}