using Entities;
using StatusEffects.Effects;
using UnityEngine;

namespace StatusEffects.Applicators
{
    public class HelStatusEffectApplicator : StatusEffectApplicator
    {
        [SerializeField] private float _movementSlowdown    = 0.05f;
        [SerializeField] private float _attackSpeedSlowdown = 0.05f;
        
        protected override void ApplyEffect(Entity entity)
        {
            var chillEffect = new StatusEffect_Chill(_duration, _stackable, _refresh);
            entity.ApplyStatusEffect(chillEffect);
        }
    }
}