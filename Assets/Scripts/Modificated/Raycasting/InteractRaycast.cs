using UnityEngine;

public class InteractRaycast : AbstractRaycast
{
    [SerializeField] float _intRadius = 0.3f;

    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, transform.forward);

        return _isHitted = Physics.SphereCast(_ray, _intRadius, out _hit, _rayDistance, _affectedLayer);
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
        Gizmos.DrawWireSphere(_hit.point, _intRadius);
    }
}