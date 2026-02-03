using UnityEngine;

namespace Abilities.Attacks
{
    public class FollowAttack : Attack
    {
        private bool _isDead;
        
        protected override void OnTriggerEnter(Collider otherCollider)
        {
            if (_isDead)
                return;
            
            _isDead = true;
            
            base.OnTriggerEnter(otherCollider);

            OnAttackFinished?.Invoke(_owner);
        }
    }
}