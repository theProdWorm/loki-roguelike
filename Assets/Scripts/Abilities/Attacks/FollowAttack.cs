using UnityEngine;

namespace Abilities.Attacks
{
    public class FollowAttack : Attack
    {
        private bool _isDead;
        
        protected override void OnTriggerEnter(Collider otherCollider)
        {
            base.OnTriggerEnter(otherCollider);

            OnAttackFinished?.Invoke();
        }
    }
}