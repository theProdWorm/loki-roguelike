using Abilities.Attacks;
using Entities;
using StatusEffects.Enums;
using UnityEngine;

namespace StatusEffects
{
    [RequireComponent(typeof(Attack))]
    public abstract class StatusEffectApplicator : MonoBehaviour
    {
        [Tooltip("Sets the amount of stacks to apply on hit.")]
        [SerializeField] protected int _stacksToApply = 1;
        
        [SerializeField] protected float _duration = 2f;
        [SerializeField] protected bool  _stackable = true;
        [Tooltip("Whether to reset the duration of all other stacks when a new one is applied.")]
        [SerializeField] protected bool  _refresh = true;
        
        private void Awake()
        {
            var attack = GetComponent<Attack>();
            attack.OnHitEntity.AddListener(ApplyEffect);
        }

        protected abstract void ApplyEffect(Entity entity);
    }
}