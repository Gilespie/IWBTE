 using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraZone : MonoBehaviour
{
    [SerializeField] CameraPointFollow _camera;

    [Header("Override Position")]
    [SerializeField] Transform _overridePositionPoint;

    [Header("Override Look Target")]
    [SerializeField] Transform _overrideLookTarget;

    [Header("Behaviour Flags")]
    [SerializeField] bool _overridePositionAndLook = false;
    [SerializeField] bool _overridePositionOnly = false;
    [SerializeField] bool _overrideLookOnly = false;
    [SerializeField] bool _isTriggerOnce = false;

    [Header("Camera Offset")]
    [SerializeField] bool _overrideCameraOffset = false;
    [SerializeField] Vector3 _offset;
    [Tooltip("Time to smooth the camera offset. -1 = keep current camera value")]
    [SerializeField] float _offsetSmoothTime = 1f;

    [Header("Follow Rate")]
    [SerializeField] bool _overrideFollowRate = false;
    [SerializeField] float _followRate = 1.5f;

    [Header("Temporary Look")]
    [SerializeField] bool _lookAtTargetTemporarily = false;
    [SerializeField] float _timeToView = 3f;

    bool _alreadyTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (_isTriggerOnce && _alreadyTriggered) return;

        if (!other.TryGetComponent(out Character character)) return;

        if (_overridePositionAndLook)
        {
            _camera.SetOverridePosition(_overridePositionPoint);
            _camera.SetTargetView(_overrideLookTarget);
        }

        if (_overridePositionOnly)
        {
            _camera.SetOverridePosition(_overridePositionPoint);
        }

        if (_overrideLookOnly)
        {
            _camera.SetTargetView(_overrideLookTarget);
        }

        if (_overrideCameraOffset)
        {
            _camera.SetCameraOffset(_offset, _offsetSmoothTime);
        }

        if (_overrideFollowRate)
        {
            _camera.SetFollowRate(_followRate);
        }

        if (_lookAtTargetTemporarily)
        {
            _camera.LookAtTargetTemporary(_overrideLookTarget, _timeToView);
        }

        if (_isTriggerOnce)
            _alreadyTriggered = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (_isTriggerOnce) return;

        if (other.TryGetComponent(out Character character))
        {
            _camera.ResetToFollow();
        }
    }
}