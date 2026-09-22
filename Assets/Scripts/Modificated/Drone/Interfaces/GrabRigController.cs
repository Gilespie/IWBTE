using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabRigController : MonoBehaviour
{
    [SerializeField] Rig _grabRig;
    [SerializeField] Transform _leftHandTarget;
    [SerializeField] Transform _rightHandTarget;
    [SerializeField] private float _blendTime = 0.25f;
    [SerializeField] private float _followSpeed = 10f;

    private IGrabbable _currentGrabbable;
    private Coroutine _blendRoutine;
    private bool _isActive;

    public void Activate(IGrabbable grabbable)
    {
        _currentGrabbable = grabbable;
        _isActive = true;

        _leftHandTarget.position = grabbable.LeftHandPoint.position;
        _leftHandTarget.rotation = grabbable.LeftHandPoint.rotation;
        _rightHandTarget.position = grabbable.RightHandPoint.position;
        _rightHandTarget.rotation = grabbable.RightHandPoint.rotation;

        StartBlend(1f);
    }

    public void Deactivate()
    {
        _isActive = false;
        _currentGrabbable = null;
        StartBlend(0f);
    }

    private void LateUpdate()
    {
        if (!_isActive || _currentGrabbable == null) return;


        _leftHandTarget.position = Vector3.Lerp(_leftHandTarget.position, _currentGrabbable.LeftHandPoint.position, _followSpeed * Time.deltaTime);
        _rightHandTarget.position = Vector3.Lerp(_rightHandTarget.position, _currentGrabbable.RightHandPoint.position, _followSpeed * Time.deltaTime);
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
