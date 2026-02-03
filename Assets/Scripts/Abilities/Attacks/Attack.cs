using Entities;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Abilities.Attacks
{
    public abstract class Attack : MonoBehaviour
    {
        public UnityEvent<Entity> OnHitEntity;
        public UnityEvent OnAttackFinished;
        
        protected Entity _owner;
        
        protected AttackStats _stats;

        protected string _hostileTag;

        public void SetStats(AttackStats stats) => _stats = stats;

        public void SetOwner(Entity owner) 
        {
            _owner = owner;
            tag = owner.tag;
            
            if (CompareTag("Player"))
                _hostileTag = "Hostile";
            else if (CompareTag("Hostile"))
                _hostileTag = "Player";
            else if (CompareTag("Charmed"))
                _hostileTag = "Hostile";
        }
        
        public virtual void DestroySelf() => Destroy(gameObject);

        protected Entity PerformAttack(Collider otherCollider)
        {
            if (!otherCollider.CompareTag(_hostileTag) && !otherCollider.CompareTag("Charmed"))
                return null;
            
            var entity = otherCollider.gameObject.GetComponent<Entity>();
            entity.TakeDamage(_stats.Damage);
            
            OnHitEntity?.Invoke(entity);
            
            return entity;
        }
        
        protected virtual void OnTriggerEnter(Collider otherCollider)
        {
        }
    }
}