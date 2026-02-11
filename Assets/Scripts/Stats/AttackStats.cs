using UnityEngine;

namespace Stats
{
    public class AttackStats
    {
        public readonly GameObject Prefab;

        public readonly int Damage;

        public readonly float CritChance;
        public readonly float CritDamage;

        public readonly float AreaSizeMultiplier;

        public AttackStats(GameObject prefab, int damage, float critChance, float critDamage, float areaSizeMultiplier)
        {
            Prefab = prefab;
            Damage = damage;
            CritChance = critChance;
            CritDamage = critDamage;
            AreaSizeMultiplier = areaSizeMultiplier;
        }

        public AttackStats(GameObject prefab, AttackStats original) :
            this(prefab,
                original.Damage, 
                original.CritChance, 
                original.CritDamage,
                original.AreaSizeMultiplier)
        {
        }
    }
}