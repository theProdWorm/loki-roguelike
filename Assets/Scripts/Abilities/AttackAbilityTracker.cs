using System;
using System.Collections.Generic;
using Entities.Player;
using Items;
using Stats;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Abilities
{
    public class AttackAbilityTracker : AbilityTracker
    {
        private readonly Action<AbilityStats, int> _onAbilityUsed;
        
        public AttackAbilityTracker(Ability ability, Action<AbilityStats, int> onAbilityUsed) 
            : base(ability)
        {
            _onAbilityUsed = onAbilityUsed;
        }

        public override void RegisterInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_remainingCharges == 0)
                    return;

                if (_ability.AbilityStats.Count == 1 && TryUse(out var stats, out int useTimes))
                    _onAbilityUsed(stats, useTimes);
                else
                    _holdingInput = true;
            }
            else if (context.canceled && _holdingInput)
            {
                _holdingInput = false;
                
                if (TryUse(out AbilityStats stats, out int useTimes))
                    _onAbilityUsed(stats, useTimes);
                
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
            
            if (_ability.SimultaneousRecharge || _remainingCooldown <= 0)
                _remainingCooldown = stats.RechargeTime;
            
            return true;
        }
    }
}