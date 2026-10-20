using System.Collections;
using UnityEngine;

public class CityElecticityFall : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] string _colorProperty = "_EmissionColor";
    [SerializeField] AnimationCurve _curve;
    [SerializeField] float _duration = 2f;
    bool _isDone;
    Coroutine _routine;
    int _colorId;
    Color _baseColor;

    void Awake()
    {
        _colorId = Shader.PropertyToID(_colorProperty);
        _baseColor = _mat.GetColor(_colorId);
    }

    void OnDestroy()
    {
        if (_mat != null)
            _mat.SetColor(_colorId, _baseColor);
    }

    public void TurnOff()
    {
        if (_isDone || _routine != null) return;

        _routine = StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        float time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            Apply(_curve.Evaluate(time / _duration));
            yield return null;
        }

        Apply(0f);

        _isDone = true;
        _routine = null;
    }

    void Apply(float value)
    {
        _mat.SetColor(_colorId, _baseColor * value);
    }
}