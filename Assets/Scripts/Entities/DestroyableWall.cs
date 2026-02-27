namespace Entities
{
    public class DestroyableWall : Entity
    {
        public override int TakeDamage(int _)
        {
            _currentHealth--;
            return 1;
        }
    }
}