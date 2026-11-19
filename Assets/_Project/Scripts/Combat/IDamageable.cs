namespace Isometric2DGame.Combat
{
    public interface IDamageable
    {
        public bool IsAlive { get; }
        public void TakeDamage(int value);
    }
}
