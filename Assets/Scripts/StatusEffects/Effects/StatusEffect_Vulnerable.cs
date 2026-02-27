using Entities;

namespace StatusEffects.Effects
{
    public class StatusEffect_Vulnerable : StatusEffect
    {
        private readonly float _damageTakenMultiplier;

        public StatusEffect_Vulnerable(float duration, bool stackable, bool refreshOnApplication,
            float damageTakenMultiplier) :
            base(duration, stackable, refreshOnApplication)
        {
            _damageTakenMultiplier = damageTakenMultiplier;
        }

        public override void Apply(Entity entity)
        {
            entity.AddDamageTakenMultiplier(_damageTakenMultiplier);
        }
        
        public override void Remove(Entity entity)
        {
            entity.RemoveDamageTakenMultiplier(_damageTakenMultiplier);
        }
    }
}