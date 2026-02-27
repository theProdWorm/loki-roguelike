using Entities;
using StatusEffects.Effects;
using UnityEngine;

namespace StatusEffects.Applicators
{
    public class HelStatusEffectApplicator : StatusEffectApplicator
    {
        [SerializeField] private float _slowAmount = 0.05f;
        
        protected override void ApplyEffect(Entity entity, int _)
        {
            var chillEffect = new StatusEffect_Chill(_duration, _stackable, _refresh, _slowAmount);
            entity.ApplyStatusEffect(chillEffect);
        }
    }
}