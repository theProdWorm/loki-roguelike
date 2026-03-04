using Entities;
using StatusEffects.Effects;
using UnityEngine;

namespace StatusEffects.Applicators
{
    public class HelStatusEffectApplicator : StatusEffectApplicator
    {
        [SerializeField] private float _slowAmount = 0.05f;
        
        [SerializeField] private float _frozenDuration;
        [SerializeField] private bool  _frozenStackable;
        [SerializeField] private bool  _frozenRefresh;
        
        protected override void ApplyEffect(Entity entity, int _)
        {
            var chillEffect = new StatusEffect_Chill(_duration, _stackable, _refresh, _slowAmount);
            entity.ApplyStatusEffect(chillEffect);
        }
    }
}