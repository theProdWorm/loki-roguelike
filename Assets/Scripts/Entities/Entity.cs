using System.Collections;
using Entities.Stats;
using StatusEffects;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] public EntityBaseStats EntityBaseStats;

        public UnityEvent<Entity> OnDeath;
        
        public UnityEvent<int> OnDamageTaken;
        public UnityEvent<Entity, int> OnDamageDealt;
        
        protected int _baseMaxHealth;
        protected float _baseMoveSpeed;

        protected float _speedMultiplier = 1f;
        
        protected int _maxHealth;
        protected int _damage;
        protected float _moveSpeed;
        
        protected int _currentHealth;
        
        protected float _damageTakenMultiplier = 1f;

        protected Vector3 _knockbackForce;
        private Coroutine _knockbackCoroutine;
        
        protected bool _isDead;
        
        private StatusEffectList _statusEffects;

        protected virtual void Awake()
        {
            _statusEffects = new StatusEffectList(this);
            _isDead = false;
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
            _moveSpeed = _baseMoveSpeed * _speedMultiplier;
        }
        public void RemoveSpeedMultiplier(float amount)
        {
            _speedMultiplier -= amount;
            _moveSpeed = _baseMoveSpeed * _speedMultiplier;
        }

        protected void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            
            OnDeath?.Invoke(this);
        }

        public void KnockBack(Vector3 direction, float force, float duration)
        {
            if (_knockbackCoroutine != null)
            {
                StopCoroutine(_knockbackCoroutine);
                _knockbackCoroutine = null;
            }
            
            _knockbackForce = direction * force;
            _knockbackCoroutine = StartCoroutine(KnockBackFadeCoroutine(direction, force, duration));
        }

        private IEnumerator KnockBackFadeCoroutine(Vector3 direction, float originalForce, float duration)
        {
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                float force = originalForce * Mathf.Abs(Mathf.Pow(t, 3) - 1);
                _knockbackForce = force * direction;
                
                yield return null;
            }
            
            _knockbackForce = Vector3.zero;
            _knockbackCoroutine = null;
        }
    }
}