using UnityEngine;

namespace Abilities.Attacks
{
    public class AreaAttack : Attack
    {
        private Vector3 _baseScale;
        
        private float _areaSizeMultiplier = 1f;

        public float AreaSizeMultiplier
        {
            get => _areaSizeMultiplier;
            set
            {
                _areaSizeMultiplier = value;
                UpdateScale();
            }
        }

        private void Awake()
        {
            _baseScale = transform.localScale;
            UpdateScale();
        }
        
        private void UpdateScale() => transform.localScale = _baseScale * AreaSizeMultiplier;

        protected override void OnTriggerEnter(Collider otherCollider)
        {
            PerformAttack(otherCollider);
        }
    }
}