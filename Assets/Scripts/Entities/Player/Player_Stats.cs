using System.Collections.Generic;
using Entities.Stats;
using Items;
using UnityEngine;

namespace Entities.Player
{
    public partial class Player : Entity
    {
        [HideInInspector]
        public PlayerBaseStats PlayerBaseStats;

        private float _splashRadiusMultiplier;
        
        private float _critChance;
        private float _critDamage;
        
        protected float _damageReduction = 0f;
        
        private readonly List<IItem> _items = new();
        
        private readonly List<Effect> _effects = new();

        protected override void InitializeBaseStats()
        {
            base.InitializeBaseStats();

            _critChance = PlayerBaseStats.CritChance;
            _critDamage = PlayerBaseStats.CritDamage;
        }
        
        public override void TakeDamage(int amount)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            base.TakeDamage(reducedDamage);
        }
        
        public void AddItem(IItem item)
        {
            _items.Add(item);
            
            item.Apply(this);
        }

        #region Add Base Stats
        public void AddBaseMaxHealth(int amount)
        {
            _baseMaxHealth += amount;
        }

        public void AddBaseDamage(int amount)
        {
            _baseDamage += amount;
        }

        public void AddBaseMoveSpeed(float amount)
        {
            _baseMoveSpeed += amount;
        }
        #endregion
        
        
        public void AddMaxHealthMultiplier(int amount)
        {
            _maxHealthMultiplier += amount;
        }

        public void AddDamageMultiplier(int amount)
        {
            _damageMultiplier += amount;
        }

        public void AddRangeMultiplier(float amount)
        {
            _rangeMultiplier += amount;
        }

        public void AddMoveSpeedMultiplier(float amount)
        {
            _moveSpeedMultiplier += amount;
        }

        public void AddCritChanceMultiplier(float amount)
        {
            _critChance += amount;
        }

        public void AddCritDamageMultiplier(float amount)
        {
            _critDamage += amount;
        }

        public void AddDamageReductionMultiplier(float amount)
        {
            _damageReduction += amount;
        }
    }
}
