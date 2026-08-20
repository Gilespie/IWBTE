using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimController : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint[] _aim;
    [SerializeField] private Rig _rig;
    [SerializeField] private AimTarget _aimTarget;
    [SerializeField] private float _blendTime = 0.3f;

    private Coroutine _blendRoutine;

    public void SetTarget(Transform target)
    {
        float targetWeight = target != null ? 1f : 0f;

        if (_blendRoutine != null)
            StopCoroutine(_blendRoutine);

        _blendRoutine = StartCoroutine(BlendRoutine(targetWeight));
    }

    public void SetAimPosition(Transform target)
    {
        _aimTarget.SetPosition(target);
    }

    private IEnumerator BlendRoutine(float targetWeight)
    {
        float startWeight = _rig.weight;
        float time = 0f;

        while (time < _blendTime)
        {
            time += Time.deltaTime;

            float t = time / _blendTime;
            _rig.weight = Mathf.Lerp(startWeight, targetWeight, t);

            yield return null;
        }

        _rig.weight = targetWeight;
    }
} 