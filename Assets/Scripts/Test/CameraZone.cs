 using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
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
    [SerializeField] bool _overrideZPosition = false;
    [SerializeField] bool _overrideCameraOffset = false;
    [SerializeField] bool _isTriggerOnce = false;
    [SerializeField] bool _isLerpingSpeed = false;
    bool _alreadyTriggered;

    [SerializeField] bool _lookAtTargetTemporarily = false;
    [SerializeField] float _timeToView = 3f;
    [SerializeField] float _zPos = -8f;
    [SerializeField] Vector3 _offset; 

    private void OnTriggerEnter(Collider other)
    {
        if (_isTriggerOnce && _alreadyTriggered) return;

        if (other.TryGetComponent(out Character character))
        {
            if (_overridePositionAndLook)
            {
                _camera.SetOverridePosition(_overridePositionPoint);
                _camera.SetTargetView(_overrideLookTarget);
            }
            if(_overridePositionOnly)
            {
                _camera.SetOverridePosition(_overridePositionPoint);
            }
            if (_overrideLookOnly)
            {
                _camera.SetTargetView(_overrideLookTarget);
            }
            if(_overrideZPosition)
            {
                _camera.SetZPos(_zPos);
            }

            if (_overrideCameraOffset)
            {
                _camera.SetCameraOffset(_offset);
            }

            if (_isTriggerOnce)
                _alreadyTriggered = true;

            if (_lookAtTargetTemporarily)
            {
                _camera.LookAtTargetTemporary(_overrideLookTarget, _timeToView);
            }

            if(_isLerpingSpeed)
            {
                _camera.ActiveLerpSmoothing(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isTriggerOnce) return;

        if (other.TryGetComponent(out Character character))
        {
            _camera.ResetToFollow();
        }
    }
}