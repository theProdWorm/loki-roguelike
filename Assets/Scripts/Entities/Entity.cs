using Entities.Stats;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected EntityBaseStats _entityBaseStats;

        protected int _maxHealth;
        protected int _currentHealth;
        
        protected float _moveSpeed;
        
        protected int _damage;
        
        private bool _isDead;

        protected virtual void Start()
        {
            StatsUpdated();
        }

        protected virtual void StatsUpdated()
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
        }
    }
}