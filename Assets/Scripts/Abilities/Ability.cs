using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        [Tooltip("If enabled, regains all charges at once when cooldown ends.")]
        public bool SimultaneousRecharge;
        public int  Charges;
        
        [FormerlySerializedAs("AbilityStats")]
        public List<AbilitySelector> Stages;

        public AbilityStats GetStats(float inputDuration)
        {
            for (int i = Stages.Count - 1; i >= 0; i--)
            {
                if (inputDuration >= Stages[i].InputTime)
                    return Stages[i].Stats;
            }

            return null;
        }
    }
}