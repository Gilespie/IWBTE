using UnityEngine;

public class ClimbingRaycast : AbstractRaycast
{
    [SerializeField] float _maxHeight = 2f;
    public Vector3 LedgePoint;

    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, direction);

        if (Physics.Raycast(_ray, out _hit, _rayDistance, _affectedLayer))
        {
            Vector3 topPoint = _hit.point + Vector3.up * _maxHeight;

            if (Physics.Raycast(topPoint, Vector3.down, out RaycastHit ledgeHit, _maxHeight))
            {
                LedgePoint = ledgeHit.point;

                return _isHitted = true;
            }
        }

        return _isHitted = false;
    }
}