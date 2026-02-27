using Entities.Stats;
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

        protected float _speedMultiplier = 1f;
        
        protected int _maxHealth;
        protected int _damage;
        protected float _moveSpeed;
        
        protected int _currentHealth;
        
        protected float _damageTakenMultiplier = 1f;
        
        public bool IsDead = false;
        
        private StatusEffectList _statusEffects;

        protected virtual void Awake()
        {
            _statusEffects = new StatusEffectList(this);
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
            _maxHealth = _baseMaxHealth;
            
            _damage = EntityBaseStats.Damage;
            
            _baseMoveSpeed = EntityBaseStats.MoveSpeed;
            _moveSpeed = _baseMoveSpeed;
            
            _currentHealth = _baseMaxHealth;
        }
        
        public virtual int TakeDamage(int amount, Entity attacker)
        {
            int realDamage = Mathf.CeilToInt(amount * _damageTakenMultiplier);
            _currentHealth -= realDamage;
            
            OnDamageTaken?.Invoke(realDamage);
            
            if (_currentHealth <= 0)
                Die();
            
            return realDamage;
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

        public void AddDamageTakenMultiplier(float amount) =>
            _damageTakenMultiplier += amount;
        public void RemoveDamageTakenMultiplier(float amount) =>
            _damageTakenMultiplier -= amount;

        public void AddSpeedMultiplier(float amount)
        {
            _speedMultiplier += amount;
            Debug.Log(_speedMultiplier);
            _moveSpeed = _baseMoveSpeed * _speedMultiplier;
        }
        public void RemoveSpeedMultiplier(float amount)
        {
            _speedMultiplier -= amount;
            Debug.Log(_speedMultiplier);
            _moveSpeed = _baseMoveSpeed * _speedMultiplier;
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            
            OnDeath?.Invoke(this);
        }
    }
}