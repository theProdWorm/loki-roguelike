using Entities.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] public EntityBaseStats EntityBaseStats;

        public UnityEvent<Entity> OnDeath;
        
        protected int _baseMaxHealth;
        protected int _baseDamage;
        protected float _baseMoveSpeed;
        
        protected int _maxHealthMultiplier;
        protected int _damageMultiplier;
        protected float _moveSpeedMultiplier;
        
        protected int _maxHealth;
        protected int _damage;
        protected float _moveSpeed;
        
        protected float _rangeMultiplier = 1f;
        
        protected int _currentHealth;
        
        private bool _isDead;

        protected virtual void InitializeBaseStats()
        {
            _baseMaxHealth = EntityBaseStats.MaxHealth;
            _baseDamage = EntityBaseStats.Damage;
            _baseMoveSpeed = EntityBaseStats.MoveSpeed;
            
            _maxHealth = EntityBaseStats.MaxHealth;
            _damage = EntityBaseStats.Damage;
            _moveSpeed = EntityBaseStats.MoveSpeed;
            
            _currentHealth = _maxHealth;
        }
        
        public virtual void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            
            if (_currentHealth <= 0)
                Die();
        }
        
        private void Die()
        {
            if (_isDead)
                return;
            
            _isDead = true;
            
            OnDeath?.Invoke(this);
        }
    }
}