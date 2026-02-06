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

        [SerializeField] private float _damageMultiplier;

        private int _damage;

        private AttackStats _stats;

        private Entity _owner;
        private string _hostileTag;

        public void SetStats(AttackStats stats) 
        {
            _stats = stats;
            _damage = Mathf.CeilToInt(_stats.Damage * _damageMultiplier);
        }

        public void SetOwner(Entity owner) 
        {
            _owner = owner;
            tag = owner.tag;
            
            if (_owner.CompareTag("Player"))
                _hostileTag = "Hostile";
            else if (_owner.CompareTag("Hostile"))
                _hostileTag = "Player";
            else if (_owner.CompareTag("Charmed"))
                _hostileTag = "Hostile";
        }
        
        public virtual void DestroySelf() => Destroy(gameObject);

        protected Entity PerformAttack(Collider otherCollider)
        {
            if (!otherCollider.CompareTag(_hostileTag) && !otherCollider.CompareTag("Charmed"))
                return null;
            
            var entity = otherCollider.gameObject.GetComponent<Entity>();
            entity.TakeDamage(_damage, _owner);
            
            OnHitEntity?.Invoke(entity);
            
            return entity;
        }

        protected abstract void OnTriggerEnter(Collider otherCollider);
    }
}