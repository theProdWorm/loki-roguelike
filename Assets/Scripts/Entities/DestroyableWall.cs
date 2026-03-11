namespace Entities
{
    public class DestroyableWall : Entity
    {
        public override int TakeDamage(int _, Entity __)
        {
            _currentHealth--;

            if (_currentHealth <= 0)
                Destroy(gameObject);
            
            return 1;
        }
    }
}