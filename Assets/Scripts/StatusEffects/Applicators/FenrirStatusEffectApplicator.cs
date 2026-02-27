using Entities;
using StatusEffects.Effects;
using UnityEngine;

namespace StatusEffects.Applicators
{
    public class FenrirStatusEffectApplicator : StatusEffectApplicator
    {
        [Space]
        [Tooltip("Amount of Wounds required to apply Vulnerable")]
        [SerializeField] private float _maxWounds;

        [SerializeField] private float _vulnerableDuration;
        [SerializeField] private bool  _vulnerableStackable;
        [SerializeField] private bool  _vulnerableRefresh;
        [SerializeField] private float _vulnerableDamageIncrease = 0.5f;
        
        protected override void ApplyEffect(Entity entity)
        {
            for (int i = 0; i < _stacksToApply; i++)
            {
                var woundEffect = new StatusEffect_Wounds(_duration, _stackable, _refresh);
                entity.ApplyStatusEffect(woundEffect);

                int count = entity.CountStatusEffectsOfType(woundEffect);
                if (count < _maxWounds)
                    continue;
                
                var vulnerableEffect = new StatusEffect_Vulnerable(
                    _vulnerableDuration, _vulnerableStackable, _vulnerableRefresh, _vulnerableDamageIncrease);
                entity.ApplyStatusEffect(vulnerableEffect);
                
                entity.RemoveAllStatusEffectsOfType(woundEffect);
            }
        }
    }
}