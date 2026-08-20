using UnityEngine;

public abstract class AbstractRaycast : MonoBehaviour
{
    [SerializeField] protected Transform _originPoint;
    [SerializeField] protected float _rayDistance = 0.45f;
    [SerializeField] protected LayerMask _affectedLayer;
    protected RaycastHit _hit;
    protected Ray _ray;
    protected bool _isHitted;

    public virtual bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, direction);

        return _isHitted = Physics.Raycast(_ray, _rayDistance, _affectedLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = _isHitted ? Color.green : Color.red;
        Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _rayDistance);
    }
}