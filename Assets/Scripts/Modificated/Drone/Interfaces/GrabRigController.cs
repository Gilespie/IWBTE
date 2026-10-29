using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabRigController : MonoBehaviour
{
    [SerializeField] Rig _grabRig;
    [SerializeField] Transform _leftHandTarget;
    [SerializeField] Transform _rightHandTarget;
    [SerializeField] private float _blendTime = 0.25f;
    [SerializeField] private float _followSpeed = 15f;

    private IGrabbable _currentGrabbable;
    private Coroutine _blendRoutine;
    private bool _isActive;

    public void Activate(IGrabbable grabbable)
    {
        _currentGrabbable = grabbable;
        _isActive = true;

        _leftHandTarget.SetPositionAndRotation(grabbable.LeftHandPoint.position, grabbable.LeftHandPoint.rotation);
        _rightHandTarget.SetPositionAndRotation(grabbable.RightHandPoint.position, grabbable.RightHandPoint.rotation);

        StartBlend(1f);
    }

    public void Deactivate()
    {
        _isActive = false;
        _currentGrabbable = null;
        StartBlend(0f);
    }

    private void Update()
    {
        if (!_isActive || _currentGrabbable == null) return;

        float k = 1f - Mathf.Exp(-_followSpeed * Time.deltaTime);

        Follow(_leftHandTarget, _currentGrabbable.LeftHandPoint, k);
        Follow(_rightHandTarget, _currentGrabbable.RightHandPoint, k);
    }

    private static void Follow(Transform target, Transform point, float k)
    {
        target.position = Vector3.Lerp(target.position, point.position, k);
        target.rotation = Quaternion.Slerp(target.rotation, point.rotation, k);
    }

    private void StartBlend(float target)
    {
        if (_blendRoutine != null) StopCoroutine(_blendRoutine);
        _blendRoutine = StartCoroutine(BlendRoutine(target));
    }

    private IEnumerator BlendRoutine(float targetWeight)
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
