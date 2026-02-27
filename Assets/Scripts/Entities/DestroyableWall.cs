namespace Entities
{
    public class DestroyableWall : Entity
    {
        public override int TakeDamage(int _, Entity __)
        {
            _currentHealth--;
            return 1;
        }
    }
}