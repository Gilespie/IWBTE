using UnityEngine;

public class CharacterRotator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float _speedRot = 10f;
    [SerializeField] private Transform _mesh;
    public Transform Mesh => _mesh;
    private SlopeRaycast _slopeRaycast;
    bool _isActive = true;
    private Vector3 _lastSwimForward = Vector3.forward;

    public void Initialize(SlopeRaycast slopeRaycast)
    {
        _slopeRaycast = slopeRaycast;
    }

    public void Rotate(Vector3 dir, Vector3 velocity)
    {
        if (!_isActive) return;

        if (_slopeRaycast.IsRaycasting(-Vector3.up))
        {
            RotateOnSlope(velocity);
        }
        else
        {
            RotateDefault(dir);
        }
    }

    private void RotateDefault(Vector3 dir)
    {
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        _mesh.rotation = Quaternion.Slerp(_mesh.rotation, targetRotation, _speedRot * Time.fixedDeltaTime);
    }

    private void RotateOnSlope(Vector3 velocity)
    {
        Vector3 slopeNormal = _slopeRaycast.Normal;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 adjustedForward = Vector3.ProjectOnPlane(velocity, slopeNormal).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(adjustedForward, slopeNormal);

        _mesh.rotation = Quaternion.Slerp(_mesh.rotation, targetRotation, Time.fixedDeltaTime * _speedRot);
    }

    public void RotateSwimming(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);

            _mesh.rotation = Quaternion.Slerp(
                _mesh.rotation,
                targetRotation,
                _speedRot * Time.fixedDeltaTime
            );

            Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);

            if (flatDir.sqrMagnitude > 0.0001f)
            {
                _lastSwimForward = flatDir.normalized;
            }
        }
        else
        {
            if (_lastSwimForward.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(_lastSwimForward);

            _mesh.rotation = Quaternion.Slerp(
                _mesh.rotation,
                targetRotation,
                _speedRot * Time.fixedDeltaTime
            );
        }
    }

    public void ToggleComponent()
    {
        _isActive = !_isActive;
    }
}