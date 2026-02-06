using UnityEngine;

namespace Abilities
{
    public class CharacterAbilityRecord
    {
        private readonly bool  SimultaneousAttackRecharge;
        private readonly int   MaxAttackCharges;
        private readonly float MaxAttackCooldown;
        private float          RemainingAttackCooldown;
        private int            RemainingAttackCharges;
        
        private readonly bool  SimultaneousSpecialRecharge;
        private readonly int   MaxSpecialCharges;
        private readonly float MaxSpecialCooldown;
        private float          RemainingSpecialCooldown;
        private int            RemainingSpecialCharges;
        
        private readonly bool  SimultaneousDashRecharge;
        private readonly int   MaxDashCharges;
        private readonly float MaxDashCooldown;
        private float          RemainingDashCooldown;
        private int            RemainingDashCharges;
        
        public CharacterAbilityRecord(CharacterAbilitySet abilities)
        {
            var attack            = abilities.Attack;
            var special = abilities.Special;
            var dash        = abilities.Dash;
            
            SimultaneousAttackRecharge  = attack.SimultaneousRecharge;
            SimultaneousSpecialRecharge = special.SimultaneousRecharge;
            SimultaneousDashRecharge    = dash.SimultaneousRecharge;
            
            MaxAttackCharges  = attack.Charges;
            MaxSpecialCharges = special.Charges;
            MaxDashCharges    = dash.Charges;

            MaxAttackCooldown  = attack.RechargeTime;
            MaxSpecialCooldown = special.RechargeTime;
            MaxDashCooldown    = dash.RechargeTime;
            
            RemainingAttackCharges  = MaxAttackCharges;
            RemainingSpecialCharges = MaxSpecialCharges;
            RemainingDashCharges    = MaxDashCharges;
        }
        
        public void Update()
        {
            if (RemainingAttackCooldown > 0)
            {
                RemainingAttackCooldown -= Time.deltaTime;

                if (RemainingAttackCooldown <= 0)
                {
                    if (SimultaneousAttackRecharge)
                        RemainingAttackCharges = MaxAttackCharges;
                    else
                        RemainingAttackCharges++;
                    
                    if (RemainingAttackCharges != MaxAttackCharges)
                        RemainingAttackCooldown = MaxAttackCooldown;
                }
            }
            
            if (RemainingSpecialCooldown > 0)
            {
                RemainingSpecialCooldown -= Time.deltaTime;
                
                if (RemainingSpecialCooldown <= 0)
                {
                    if (SimultaneousSpecialRecharge)
                        RemainingSpecialCharges = MaxSpecialCharges;
                    else
                        RemainingSpecialCharges++;
                    
                    if (RemainingSpecialCharges != MaxSpecialCharges)
                        RemainingSpecialCooldown = MaxSpecialCooldown;
                }
            }
            
            if (RemainingDashCooldown > 0)
            {
                RemainingDashCooldown -= Time.deltaTime;
                
                if (RemainingDashCooldown <= 0)
                {
                    if (SimultaneousDashRecharge)
                        RemainingDashCharges = MaxDashCharges;
                    else
                        RemainingDashCharges++;
                    
                    if (RemainingDashCharges != MaxDashCharges)
                        RemainingDashCooldown = MaxDashCooldown;
                }
            }
        }

        public bool TryUseAttack()
        {
            if (RemainingAttackCharges == 0)
                return false;

            RemainingAttackCharges--;

            if (SimultaneousAttackRecharge || RemainingAttackCooldown <= 0)
                RemainingAttackCooldown = MaxAttackCooldown;
            
            return true;
        }

        public bool TryUseSpecial()
        {
            if (RemainingSpecialCharges == 0)
                return false;
            
            RemainingSpecialCharges--;
            
            if (SimultaneousSpecialRecharge || RemainingSpecialCooldown <= 0)
                RemainingSpecialCooldown = MaxSpecialCooldown;
            
            return true;
        }

        public bool TryUseDash()
        {
            if (RemainingDashCharges == 0)
                return false;

            RemainingDashCharges--;
            
            if (SimultaneousDashRecharge || RemainingDashCooldown <= 0)
                RemainingDashCooldown = MaxDashCooldown;
            
            return true;
        }
    }
}