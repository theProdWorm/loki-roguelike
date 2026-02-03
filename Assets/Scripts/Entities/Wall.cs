namespace Entities
{
    public class Wall : Entity
    {
        public override void TakeDamage(int _)
        {
            _currentHealth--;
        }
    }
}