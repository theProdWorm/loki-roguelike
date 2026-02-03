using UnityEngine;

namespace Abilities.Attacks
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileAttack : Attack
    {
        [SerializeField] private float _speed;
        [SerializeField] private int _maxHits;

        [SerializeField] private Rigidbody _rigidbody;
        
        private int _remainingHits;
        
        private bool _isDead;

        private void Awake()
        {
            _remainingHits = _maxHits;
            _rigidbody.linearVelocity = transform.forward * _speed;
        }
        
        private void Die()
        {
            _isDead = true;
            OnAttackFinished?.Invoke();
        }
        
        protected override void OnTriggerEnter(Collider otherCollider)
        {
            if (_isDead)
                return;
            
            var entity = PerformAttack(otherCollider);
            if (!entity)
                Die();
            
            _remainingHits--;
            
            if (_remainingHits == 0)
                Die();
        }
    }
}