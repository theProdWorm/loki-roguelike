namespace Stats
{
    public class AttackStats
    {
        public int Damage;

        public float CritChance;
        public float CritDamage;

        public float AreaSizeMultiplier;

        public AttackStats(int damage, float critChance, float critDamage, float areaSizeMultiplier)
        {
            Damage = damage;
            CritChance = critChance;
            CritDamage = critDamage;
            AreaSizeMultiplier = areaSizeMultiplier;
        }
    }
}