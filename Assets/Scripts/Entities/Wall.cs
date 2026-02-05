namespace Entities
{
    public class Wall : Entity
    {
        public override void TakeDamage(int _, Entity attacker)
        {
            _currentHealth--;
        }
    }
}