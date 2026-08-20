using UnityEngine;

public abstract class Destructable : MonoBehaviour, IDamageable
{
    [Header("Entity ID")]
    [SerializeField] protected int _id = 0;

    protected bool _isAlive = true;
    public bool IsAlive => _isAlive;

    public virtual void InstantKill(params object[] parameters)
    {
        _isAlive = false;
        this.enabled = false;
    }
}