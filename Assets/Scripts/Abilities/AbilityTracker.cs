using System;
using System.Collections.Generic;
using Entities.Player;
using Items;
using Stats;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Abilities
{
    public class AbilityTracker
    {
        private readonly Action _onAbilityUsed;
        
        protected readonly Ability _ability;
        
        protected float _remainingCooldown;
        protected int   _remainingCharges;

        protected bool  _holdingInput;
        protected float _inputDuration;
        
        public AbilityTracker(Ability ability, Action onAbilityUsed)
        {
            _ability = ability;
            
            _onAbilityUsed = onAbilityUsed;
            
            _remainingCharges = ability.Charges;
        }

        protected AbilityTracker(Ability ability)
        {
            
            _ability = ability;
        }

        public void Update()
        {
            if (_holdingInput)
            {
                _inputDuration += Time.deltaTime;
                return;
            }

            if (_remainingCharges == _ability.Charges)
                return;
            
            _remainingCooldown -= Time.deltaTime;
            if (_remainingCooldown > 0)
                return;
            
            if (_ability.SimultaneousRecharge)
            {
                _remainingCharges = _ability.Charges;
            }
            else
            {
                _remainingCharges++;

                if (_remainingCharges != _ability.Charges)
                    _remainingCooldown = _ability.AbilityStats[0].Stats.RechargeTime;
            }
        }

        public virtual bool RegisterInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_remainingCharges == 0)
                    return _holdingInput;

                if (_ability.AbilityStats.Count == 1 && TryUse())
                    _onAbilityUsed();
                else
                    _holdingInput = true;
            }
            else if (context.canceled && _holdingInput)
            {
                if (TryUse())
                    _onAbilityUsed();
                
                _inputDuration = 0;
                
                _holdingInput = false;
            }

            return _holdingInput;
        }
        
        private bool TryUse()
        {
            if (_remainingCharges == 0)
                return false;
            
            var stats = _ability.GetStats(_inputDuration);
            if (stats == null)
                return false;
            
            if (stats.RequireMaxCharges && _remainingCharges != _ability.Charges)
                return false;

            int useTimes = stats.Burst ? _ability.Charges : 1;
            _remainingCharges -= useTimes;
            
            if (_ability.SimultaneousRecharge || _remainingCooldown <= 0)
                _remainingCooldown = stats.RechargeTime;
            
            return true;
        }
    }
}