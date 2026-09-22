using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabOneHandController : MonoBehaviour
{
    [SerializeField] Rig _grabRig;
    [SerializeField] Transform _rightHandTarget;
    [SerializeField] private float _blendTime = 0.25f;
    [SerializeField] private float _followSpeed = 10f;

    private Transform _currentAnchor;
    private Coroutine _blendRoutine;
    private bool _isActive;

    public void Activate(Transform anchor)
    {
        _currentAnchor = anchor;
        _isActive = true;

        _rightHandTarget.position = anchor.position;
        _rightHandTarget.rotation = anchor.rotation;

        StartBlend(1f);
    }

    public void Deactivate()
    {
        _isActive = false;
        _currentAnchor = null;
        StartBlend(0f);
    }

    private void LateUpdate()
    {
        if (!_isActive || _currentAnchor == null) return;

        _rightHandTarget.position = Vector3.Lerp(_rightHandTarget.position, _currentAnchor.position, _followSpeed * Time.deltaTime);
        Vector3 defaultRotation = new(-306, 0, 0);
        _rightHandTarget.rotation = Quaternion.Euler(defaultRotation);
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
