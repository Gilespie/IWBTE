using UnityEngine;

public class SlopeRaycast : AbstractRaycast
{
    [SerializeField] float _maxSlopeAngle = 26.5f;
    float _currentSlopeAngle = 0f;
    Vector3 _normalOrient;
    public Vector3 Normal => _normalOrient;

    public override bool IsRaycasting(Vector3 direction)
    {
        _ray = new Ray(_originPoint.position, direction);

        if (Physics.Raycast(_ray, out _hit, _rayDistance, _affectedLayer))
        {
            _currentSlopeAngle = Vector3.Angle(_hit.normal, Vector3.up);

            if (_currentSlopeAngle >= _maxSlopeAngle)
            {
                _normalOrient = _hit.normal;
                return true;
            }
        }

        return false;
    }
}