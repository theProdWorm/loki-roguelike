using Entities;

namespace StatusEffects.Effects
{
    public class StatusEffect_Vulnerable : StatusEffect
    {
        public StatusEffect_Vulnerable(float duration, bool stackable, bool refreshOnApplication) : 
            base(duration, stackable, refreshOnApplication)
        {
            
        }
        
        public override void Apply(Entity entity)
        {
            
        }
        
        public override void Remove(Entity entity)
        {
            
        }
    }
}