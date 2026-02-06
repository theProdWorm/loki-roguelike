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
        public UnityEvent<int> OnDamageTaken;
        
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
        
        protected float _areaSizeMultiplier = 1f;
        
        protected int _currentHealth;
        
        private bool _isDead;

        private readonly List<Attack> _activeAttacks = new();

        protected virtual void Start()
        {
            InitializeBaseStats();
        }
        
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
        
        public virtual void TakeDamage(int amount, Entity attacker)
        {
            Debug.Log(_currentHealth);
            
            _currentHealth -= amount;
            
            OnDamageTaken?.Invoke(amount);
            
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

        protected void InstantiateAttack(GameObject prefab, AttackStats stats)
        {
            var attackInstance = Instantiate(prefab, transform.position, transform.rotation)
                .GetComponentInChildren<Attack>(true);
            
            attackInstance.SetOwner(this);
            attackInstance.SetStats(stats);
            
            
            if (attackInstance is AreaAttack areaAttack)
                areaAttack.AreaSizeMultiplier = _areaSizeMultiplier;
            
            _activeAttacks.Add(attackInstance);
            
            attackInstance.transform.parent.position = transform.position;
        }
    }
}