using Entities;

namespace StatusEffects.Effects
{
    public class StatusEffect_Frozen : StatusEffect
    {
        public StatusEffect_Frozen(float duration, bool stackable, bool refreshOnApplication) : 
            base(duration, stackable, refreshOnApplication)
        {
        }
        
        public override void Apply(Entity entity)
        {
            entity.RemoveSpeedMultiplier(1);
        }

        public override void Remove(Entity entity)
        {
            entity.AddSpeedMultiplier(1);
        }
    }
}