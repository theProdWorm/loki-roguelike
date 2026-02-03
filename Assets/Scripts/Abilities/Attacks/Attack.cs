using System.Collections.Generic;
using Entities;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Abilities.Attacks
{
    public abstract class Attack : MonoBehaviour
    {
        public UnityEvent<Entity> OnAttackFinished;
        public UnityEvent<Entity> OnHit;
        
        protected Entity _owner;
        
        private AttackStats _stats;

        private string _hostileTag;
        
        public void SetStats(AttackStats stats) => _stats = stats;

        public void SetOwner(Entity owner) 
        {
            _owner = owner;
            tag = owner.tag;
            
            if (tag  == "Player")
                _hostileTag = "Hostile";
            else if (tag == "Hostile")
                _hostileTag = "Player";
            else if (tag == "Charmed")
                _hostileTag = "Hostile";
        }

        protected virtual void TryHitEntity(Entity entity)
        {
            if (!entity.CompareTag(_hostileTag) && !entity.CompareTag("Charmed"))
                return;
            
            entity.TakeDamage(_stats.Damage);
            
            OnHit?.Invoke(entity);
        }
        
        protected virtual void OnTriggerEnter(Collider otherCollider)
        {
            var entity = otherCollider.gameObject.GetComponent<Entity>();
            TryHitEntity(entity);
        }
    }
}