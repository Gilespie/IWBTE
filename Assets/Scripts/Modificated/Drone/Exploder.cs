using System;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] float _force = 3000f;
    [SerializeField] float _upForce = 1f;
    [SerializeField] float _radius = 5f;
    [SerializeField] LayerMask _damageMask;
    [SerializeField] GameObject _vfx;
    [SerializeField] bool _destroyOnExplode = true;

    public event Action OnExploded;
    public bool IsExploded { get; private set; }

    public void Explode()
    {
        if (IsExploded) return;
        IsExploded = true;

        foreach (var hit in Physics.OverlapSphere(transform.position, _radius, _damageMask))
        {
            if (!hit.TryGetComponent(out IDamageable dmg)) continue;

            dmg.InstantKill();

            foreach (var rb in hit.GetComponentsInChildren<Rigidbody>())
                rb.AddExplosionForce(_force, transform.position, _radius, _upForce);
        }

        if (_vfx != null)
            Instantiate(_vfx, transform.position, Quaternion.identity);

        OnExploded?.Invoke();

        if (_destroyOnExplode)
            Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}