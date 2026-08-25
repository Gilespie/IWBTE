using UnityEngine;

public abstract class AbstractRaycast : MonoBehaviour
{
    [SerializeField] protected Transform _originPoint;
    [SerializeField] protected float _rayDistance = 0.45f;
    [SerializeField] protected float _radius = 0.1f;
    [SerializeField] protected LayerMask _affectedLayer;
    protected RaycastHit _hit;
    protected Ray _ray;
    protected bool _isHitted;

    public RaycastHit Hit => _hit;
    public bool IsHitted => _isHitted;

    public virtual bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, direction);

        return _isHitted = Physics.SphereCast(_ray, _radius, out _hit, _rayDistance, _affectedLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        if (_originPoint == null) return;

        Gizmos.color = _isHitted ? Color.green : Color.red;

        float travelDistance = _isHitted ? _hit.distance : _rayDistance;

        Vector3 sphereCenter = _ray.origin + _ray.direction.normalized * travelDistance;

        Gizmos.DrawLine(_ray.origin, sphereCenter);

        if (_radius > 0f)
        {
            Gizmos.DrawWireSphere(sphereCenter, _radius);
        }
    }
}