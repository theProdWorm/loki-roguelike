namespace Entities
{
    public class DestroyableWall : Entity
    {
        public override void TakeDamage(int _, Entity attacker)
        {
            _currentHealth--;
        }
    }
}