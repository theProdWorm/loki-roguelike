using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        [Tooltip("If enabled, regains all charges at once when cooldown ends.")]
        public bool SimultaneousRecharge;
        public int  Charges;
        
        public List<AbilitySelector> AbilityStats;

        public AbilityStats GetStats(float inputDuration)
        {
            for (int i = AbilityStats.Count - 1; i >= 0; i--)
            {
                if (inputDuration >= AbilityStats[i].InputTime)
                    return AbilityStats[i].Stats;
            }

            return null;
        }
    }
}