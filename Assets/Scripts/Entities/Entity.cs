using Entities.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected EntityBaseStats _entityBaseStats;

        public UnityEvent<Entity> OnDeath;
        
        protected int _maxHealth;
        protected int _currentHealth;
        
        protected float _moveSpeed;
        
        protected int _damage;
        
        private bool _isDead;

        private void Start()
        {
            StatsUpdated();
        }

        private void StatsUpdated()
        {
            _maxHealth = _entityBaseStats.MaxHealth;
            _moveSpeed = _entityBaseStats.MoveSpeed;
            _damage = _entityBaseStats.Damage;
            
            // TODO: Apply additional stats
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            
            if (_currentHealth <= 0)
                Die();
        }
        
        protected void Die()
        {
            if (_isDead)
                return;
            
            _isDead = true;
            OnDeath?.Invoke(this);
        }
    }
}