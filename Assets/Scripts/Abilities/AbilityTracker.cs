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
        private Ability _ability;
        
        private Action _onAbilityUsed;
        
        private float _remainingCooldown;
        private int   _remainingCharges;

        private bool  _holdingInput;
        private float _inputDuration;
        
        public AbilityTracker(Ability ability, Action onAbilityUsed)
        {
            _ability = ability;
            
            _onAbilityUsed = onAbilityUsed;
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
            
            _remainingCooldown -= Time.deltaTime;

            if (_remainingCooldown <= 0)
            {
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
        }

        public virtual void RegisterInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_remainingCharges == 0)
                    return;

                if (_ability.AbilityStats.Count == 1)
                {
                    if (TryUse(out AbilityStats stats, out int useTimes))
                    {
                        _onAbilityUsed();
                    }
                }
                else
                {
                    _holdingInput = true;
                }
            }
            else if (context.canceled && _holdingInput)
            {
                _holdingInput = false;
                
                if (TryUse(out AbilityStats stats, out int useTimes))
                    _onAbilityUsed();
                
                _inputDuration = 0;
            }
        }
        
        private bool TryUse(out AbilityStats stats, out int useTimes)
        {
            stats = null;
            useTimes = 0;
            
            if (_remainingCharges == 0)
                return false;
            
            stats = _ability.GetStats(_inputDuration);
            if (stats == null)
                return false;
            
            if (stats.RequireMaxCharges && _remainingCharges != _ability.Charges)
                return false;

            useTimes = stats.Burst ? _ability.Charges : 1;
            _remainingCharges -= useTimes;
            
            return true;
        }
    }
}