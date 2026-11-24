using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabOneHandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rig _grabRig;
    [SerializeField] Transform _rightHandTarget;
    [Tooltip("Body of player")]
    [SerializeField] Transform _bodyRoot;

    [Header("Blend / Follow")]
    [SerializeField] float _blendTime = 0.25f;
    [SerializeField] float _followSpeed = 10f;
    [Tooltip("degrees per second")]
    [SerializeField] float _rotationSpeed = 360f;

    [Header("Rotation")]
    [SerializeField] Vector3 _handRotationOffset;
    [Tooltip("If disabled — the hand maintains its orientation when grabbing the lever")]
    [SerializeField] bool _followAnchorRotation = true;
    [Tooltip("Maximum deviation of the hand from the initial orientation (in the body system)")]
    [SerializeField, Range(0f, 180f)] float _maxTwistAngle = 50f;

    Transform _currentAnchor;
    Coroutine _blendRoutine;
    bool _isActive;

    Quaternion _referenceLocalRot;

    Quaternion BodyRot => _bodyRoot != null ? _bodyRoot.rotation : Quaternion.identity;

    public void Activate(Transform anchor)
    {
        _currentAnchor = anchor;
        _isActive = true;

        Quaternion startRot = anchor.rotation * Quaternion.Euler(_handRotationOffset);

        _rightHandTarget.SetPositionAndRotation(anchor.position, startRot);

        _referenceLocalRot = Quaternion.Inverse(BodyRot) * startRot;

        StartBlend(1f);
    }

    public void Deactivate()
    {
        _isActive = false;
        _currentAnchor = null;
        StartBlend(0f);
    }

    void Update()
    {
        if (!_isActive || _currentAnchor == null) return;

        float dt = Time.deltaTime;

        float k = 1f - Mathf.Exp(-_followSpeed * dt);
        _rightHandTarget.position = Vector3.Lerp(_rightHandTarget.position, _currentAnchor.position, k);

        Quaternion reference = BodyRot * _referenceLocalRot;

        Quaternion desired = _followAnchorRotation
            ? _currentAnchor.rotation * Quaternion.Euler(_handRotationOffset)
            : reference;

        desired = Quaternion.RotateTowards(reference, desired, _maxTwistAngle);

        _rightHandTarget.rotation = Quaternion.RotateTowards(
            _rightHandTarget.rotation, desired, _rotationSpeed * dt);
    }

    void StartBlend(float target)
    {
        if (_blendRoutine != null) StopCoroutine(_blendRoutine);
        _blendRoutine = StartCoroutine(BlendRoutine(target));
    }

    IEnumerator BlendRoutine(float targetWeight)
    {
        float startWeight = _grabRig.weight;
        float time = 0f;

        while (time < _blendTime)
        {
            time += Time.deltaTime;
            _grabRig.weight = Mathf.Lerp(startWeight, targetWeight, time / _blendTime);
            yield return null;
        }

        _grabRig.weight = targetWeight;
    }
}