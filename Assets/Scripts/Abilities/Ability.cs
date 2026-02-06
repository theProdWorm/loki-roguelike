using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject
    {
        [Tooltip("If enabled, regains all charges at once when cooldown ends.")]
        public bool  SimultaneousRecharge;
        
        public int   Charges;
        public float RechargeTime;

    }
}