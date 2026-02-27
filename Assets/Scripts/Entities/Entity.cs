using System;
using Entities.Stats;
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
        protected float _baseMoveSpeed;

        protected int _maxHealth;
        protected int _damage;
        protected float _moveSpeed;
        
        protected int _currentHealth;
        
        protected float _damageTakenMultiplier = 1f;
        
        public bool IsDead = false;
        
        private StatusEffectList _statusEffects;

        protected virtual void Awake()
        {
            _statusEffects = new StatusEffectList(this, Debug.Log);
            IsDead = false;
        }

        //TODO: delete test
        protected virtual void OnEnable()
        {
            IsDead = false;
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
            _damage = EntityBaseStats.Damage;
            _baseMoveSpeed = EntityBaseStats.MoveSpeed;
            _moveSpeed = _baseMoveSpeed;
            
            _currentHealth = _baseMaxHealth;
        }
        
        public virtual void TakeDamage(int amount, Entity attacker)
        {
            _currentHealth -= amount;
            
            OnDamageTaken?.Invoke(amount);
            
            if (_currentHealth <= 0)
                Die();
        }

        public virtual void Heal(int amount)
        {
            _currentHealth += amount;

            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }

        public void ApplyStatusEffect(StatusEffect effect) => 
            _statusEffects.Add(effect);
        public void RemoveAllStatusEffectsOfType(StatusEffect sampleEffect, int max = int.MaxValue) => 
            _statusEffects.RemoveAll(sampleEffect, max);
        public int  CountStatusEffectsOfType(StatusEffect sampleEffect) => 
            _statusEffects.GetCount(sampleEffect);

        public float AddDamageTakenMultiplier(float amount) =>
            _damageTakenMultiplier += amount;
        public float RemoveDamageTakenMultiplier(float amount) =>
            _damageTakenMultiplier -= amount;
        
        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            
            OnDeath?.Invoke(this);
        }
    }
}