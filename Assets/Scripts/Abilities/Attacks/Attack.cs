using Entities;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Abilities.Attacks
{
    public abstract class Attack : MonoBehaviour
    {
        public UnityEvent<Entity> OnHitEntity;
        public UnityEvent<Entity, int> OnHitEntityWithDamage;
        public UnityEvent OnAttackFinished;

        [SerializeField] private float _damageMultiplier;

        private int _damage;

        protected AttackStats _stats;

        protected Entity _owner;
        protected string _hostileTag;

        public static void Create(Entity owner, Vector3 position, Quaternion rotation, AttackStats stats)
        {
            var attackInstance = Instantiate(stats.Prefab, position, rotation)
                .GetComponentInChildren<Attack>(true);

            attackInstance.SetOwner(owner);
            attackInstance.SetStats(stats);

            if (attackInstance is AreaAttack areaAttack)
                areaAttack.AreaSizeMultiplier = stats.AreaSizeMultiplier;

            attackInstance.transform.parent.position = position;
        }
        
        private void SetStats(AttackStats stats) 
        {
            _stats = stats;
            _damage = Mathf.CeilToInt(_stats.Damage * _damageMultiplier);
        }

        private void SetOwner(Entity owner) 
        {
            OnHitEntity.AddListener(owner.OnDamageDealt.Invoke);
            
            _owner = owner;
            tag = owner.tag;
            
            if (_owner.CompareTag("Player"))
                _hostileTag = "Hostile";
            else if (_owner.CompareTag("Hostile"))
                _hostileTag = "Player";
        }
        
        public virtual void DestroySelf() => Destroy(transform.parent.gameObject);

        protected Entity PerformAttack(Collider otherCollider)
        {
            if (!otherCollider.CompareTag(_hostileTag))
                return null;
            
            var entity = otherCollider.gameObject.GetComponent<Entity>();
            entity.TakeDamage(_damage, _owner);
            
            OnHitEntity?.Invoke(entity);
            
            return entity;
        }

        protected abstract void OnTriggerEnter(Collider otherCollider);
    }
}