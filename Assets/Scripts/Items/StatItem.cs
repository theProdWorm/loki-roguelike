using Entities.Player;
using UnityEngine;

namespace Items
{
    public class StatItem : IItem
    {
        private StatItemStats _stats;
        
        public void Apply(Player player)
        {
            if (_stats.MaxHealth != 0)
                player.AddMaxHealthMultiplier(_stats.MaxHealth);
            if (_stats.Damage != 0)
                player.AddDamageMultiplier(_stats.Damage);
            if (!Mathf.Approximately(_stats.Range, 0))
                player.AddRangeMultiplier(_stats.Range);
            if (!Mathf.Approximately(_stats.MoveSpeed, 0))
                player.AddMoveSpeedMultiplier(_stats.MoveSpeed);
            if (!Mathf.Approximately(_stats.CritChance, 0))
                player.AddCritChanceMultiplier(_stats.CritChance);
            if (!Mathf.Approximately(_stats.CritDamage, 0))
                player.AddCritDamageMultiplier(_stats.CritDamage);
            if (!Mathf.Approximately(_stats.DamageReduction, 0))
                player.AddDamageReductionMultiplier(_stats.DamageReduction);
        }
    }
}