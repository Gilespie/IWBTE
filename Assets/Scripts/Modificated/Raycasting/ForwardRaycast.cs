using UnityEngine;

public class ForwardRaycast : AbstractRaycast
{
/*    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, transform.forward);

        return _isHitted = Physics.SphereCast(_ray, _radius, out _hit, _rayDistance, _affectedLayer);
    }

    public void InteractPress()
    {
        if (!_isHitted) return;

        if (_hit.collider.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }

    private new void OnDrawGizmos()
    {
        Gizmos.color = _isHitted ? Color.green : Color.red;
        Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _rayDistance);
        Gizmos.DrawWireSphere(_hit.point, _radius);
    }*/

    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, direction);

        return _isHitted = Physics.SphereCast(_ray, _radius, out _hit, _rayDistance, _affectedLayer);
    }

    public bool TryGetHit<T>(out T component) where T : class
    {
        component = null;

        if (!_isHitted) return false;

        return _hit.collider.TryGetComponent(out component);
    }
}