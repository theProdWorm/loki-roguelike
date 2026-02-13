using Entities;

namespace StatusEffects.Effects
{
    public class StatusEffect_Chill : StatusEffect
    {
        public StatusEffect_Chill(float duration, bool stackable, bool refreshOnApplication) : 
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