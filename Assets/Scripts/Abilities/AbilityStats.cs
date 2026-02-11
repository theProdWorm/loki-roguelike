using UnityEngine;

namespace Abilities
{
    [System.Serializable]
    public class AbilityStats
    {
        public GameObject AttackPrefab;
        
        public bool  RequireMaxCharges;

        public float RechargeTime;

        [Tooltip("Whether to use all charges at once.")]
        public bool  Burst;
        [Tooltip("Time between charge uses in a burst.")]
        public float BurstDelay;

        [Tooltip("Angle in degrees between each attack in a spread.")]
        public float SpreadAngle;
    }
}