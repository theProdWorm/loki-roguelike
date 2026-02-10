using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    [CreateAssetMenu(fileName = "New StatItem", menuName = "Items/StatItem")]
    public class StatItemStats : BaseItemStats
    {
            
        public int BaseMaxHealth;
        public int BaseDamage;

        public float BaseMoveSpeed;
        
        [Range(-1, 3)]
        public float MaxHealthMultiplier;
        [Range(-1, 3)]
        public float DamageMultiplier;

        [Range(-1, 3)]
        public float MoveSpeedMultiplier;
        
        [Range(-1, 3)]
        public float AreaSizeMultiplier;
        
        [Range(-1, 1)]
        public float CritChance;
        [Range(-1, 3)]
        public float CritDamage;

        [Range(-1, 1)]
        public float DamageReduction;
    }
}