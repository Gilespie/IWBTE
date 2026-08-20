public interface IDamageable
{
    bool IsAlive { get; }
    void InstantKill(params object[] parameters);
}