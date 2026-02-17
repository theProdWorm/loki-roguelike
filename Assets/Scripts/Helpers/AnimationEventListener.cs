using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
    public class AnimationEventListener : MonoBehaviour
    {
        public UnityEvent OnAttackStarted;
        public UnityEvent OnAttackPerformed;
        public UnityEvent OnSpecialStarted;
        public UnityEvent OnSpecialPerformed;
        public UnityEvent OnSwitch;
        public UnityEvent OnDeath;
        
        private void InvokeAttackStarted() => OnAttackStarted?.Invoke();
        private void InvokeAttackPerformed() => OnAttackPerformed?.Invoke();
        private void InvokeSpecialStarted() => OnSpecialStarted?.Invoke();
        private void InvokeSpecialPerformed() => OnSpecialPerformed?.Invoke();
        private void InvokeSwitch() => OnSwitch?.Invoke();
        private void InvokeDeath() => OnDeath?.Invoke();
    }
}
