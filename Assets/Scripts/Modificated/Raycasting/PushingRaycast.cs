using UnityEngine;

public class PushingRaycast : AbstractRaycast
{
    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, transform.forward);

        return _isHitted = Physics.Raycast(_ray, out _hit, _rayDistance, _affectedLayer);
    }

    public void InteractPress()
    {
        if (!_isHitted) return;

        if (_hit.collider.TryGetComponent(out IPushable pushable))
        {
            pushable.Pushing(this);
        }
    }

    private new void OnDrawGizmos()
    {
        Gizmos.color = _isHitted ? Color.green : Color.red;
        Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _rayDistance);
    }
}