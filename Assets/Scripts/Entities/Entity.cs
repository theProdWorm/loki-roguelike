using Entities.Stats;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected EntityBaseStats _stats;

        protected int _maxHealth;
        protected int _currentHealth;
        
        protected float _moveSpeed;
        
        protected int _damage;

        private void Start()
        {
            
        }

        private void StatsUpdated()
        {
            
        }
    }
}