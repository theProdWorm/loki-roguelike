using System.Collections.Generic;
using Abilities.Attacks;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] public EntityBaseStats EntityBaseStats;

        public UnityEvent<Entity> OnDeath;
        
        [Tooltip("Referenced entity DEALT damage")]
        public UnityEvent<Entity> OnDamageTaken;
        
        [Tooltip("Referenced entity TOOK damage")]
        public UnityEvent<Entity> OnDamageDealt;
        
        protected int _baseMaxHealth;
        protected int _baseDamage;
        protected float _baseMoveSpeed;
        
        protected float _maxHealthMultiplier;
        protected float _damageMultiplier;
        protected float _moveSpeedMultiplier;
        
        protected int _maxHealth;
        protected int _damage;
        protected float _moveSpeed;
        
        protected float _AoEMultiplier = 1f;
        
        protected int _currentHealth;
        
        private bool _isDead;

        private List<Attack> _activeAttacks;
        
        protected virtual void InitializeBaseStats()
        {
            _baseMaxHealth = EntityBaseStats.MaxHealth;
            _baseDamage = EntityBaseStats.Damage;
            _baseMoveSpeed = EntityBaseStats.MoveSpeed;
            
            _maxHealth = _baseMaxHealth;
            _damage = _baseDamage;
            _moveSpeed = _baseMoveSpeed;
            
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

        private void InstantiateAttack(Attack attack)
        {
            var attackInstance = Instantiate(attack, transform.position, transform.rotation);
            attackInstance.SetOwner(this);
            
            _activeAttacks.Add(attackInstance);
        }
    }
}