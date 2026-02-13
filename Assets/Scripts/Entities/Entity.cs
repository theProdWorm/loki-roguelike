using System;
using Stats;
using StatusEffects;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] public EntityBaseStats EntityBaseStats;

        public UnityEvent<Entity> OnDeath;
        
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
        
        private StatusEffectList _statusEffects;

        protected virtual void Awake()
        {
            _statusEffects = new StatusEffectList(this, Debug.Log);
        }

        protected virtual void Start()
        {
            InitializeBaseStats();
        }

        protected virtual void Update()
        {
            _statusEffects.Update();
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
            _currentHealth -= amount;
            
            OnDamageTaken?.Invoke(amount);
            
            if (_currentHealth <= 0)
                Die();
        }

        public void ApplyStatusEffect(StatusEffect effect) => 
            _statusEffects.Add(effect);
        public void RemoveAllStatusEffectsOfType(StatusEffect sampleEffect, int max = int.MaxValue) => 
            _statusEffects.RemoveAll(sampleEffect, max);
        public int  CountStatusEffectsOfType(StatusEffect sampleEffect) => 
            _statusEffects.GetCount(sampleEffect);
        
        private void Die()
        {
            if (_isDead)
                return;
            
            _isDead = true;
            
            OnDeath?.Invoke(this);
        }
    }
}