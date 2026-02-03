using Entities.Player;
using UnityEngine;

namespace Items
{
    public class StatItem : IItem
    {
        private StatItemStats _stats;
        
        public void Apply(Player player)
        {
            if (_stats.BaseMaxHealth != 0)
                player.AddBaseMaxHealth(_stats.BaseMaxHealth);
            if (_stats.BaseDamage != 0)
                player.AddBaseDamage(_stats.BaseDamage);
            if (_stats.BaseMoveSpeed != 0)
                player.AddBaseMoveSpeed(_stats.BaseMoveSpeed);
            
            if (!Mathf.Approximately(_stats.MaxHealthMultiplier, 0))
                player.AddMaxHealthMultiplier(_stats.MaxHealthMultiplier);
            if (!Mathf.Approximately(_stats.DamageMultiplier, 0))
                player.AddDamageMultiplier(_stats.DamageMultiplier);
            if (!Mathf.Approximately(_stats.RangeMultiplier, 0))
                player.AddAreaSizeMultiplier(_stats.RangeMultiplier);
            
            if (!Mathf.Approximately(_stats.MoveSpeedMultiplier, 0))
                player.AddMoveSpeedMultiplier(_stats.MoveSpeedMultiplier);
            if (!Mathf.Approximately(_stats.CritChance, 0))
                player.AddCritChanceMultiplier(_stats.CritChance);
            if (!Mathf.Approximately(_stats.CritDamage, 0))
                player.AddCritDamageMultiplier(_stats.CritDamage);
            if (!Mathf.Approximately(_stats.DamageReduction, 0))
                player.AddDamageReductionMultiplier(_stats.DamageReduction);
        }
    }
}