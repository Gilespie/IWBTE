using UnityEngine;

public readonly struct AimTargetStruct
{
    readonly ShootPoint _marker;
    readonly Collider _collider;
    readonly Transform _fallback;

    public bool IsValid => _fallback != null;

    public AimTargetStruct(Transform t)
    {
        _fallback = t;
        _marker = t != null ? t.GetComponentInParent<ShootPoint>() : null;
        _collider = null;

        if (t != null && _marker == null)
        {
            foreach (var c in t.GetComponentsInChildren<Collider>())
            {
                if (c.enabled && !c.isTrigger) { _collider = c; break; }
            }
        }
    }

    public Vector3 Position
    {
        get
        {
            if (_marker != null) return _marker.Position;
            if (_collider != null) return _collider.bounds.center;
            return _fallback != null ? _fallback.position : Vector3.zero;
        }
    }
}