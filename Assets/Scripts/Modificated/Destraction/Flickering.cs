using UnityEngine;

public class Flickering : MonoBehaviour
{
    [SerializeField] AnimationCurve _lightIntensity;
    [SerializeField] float _maxIntensity = 5f;
    [SerializeField] float _speed;
    [SerializeField] Light _light;
    float _time;

    void Update()
    {
        _time += Time.deltaTime * _speed;

        float duration = _lightIntensity.keys[_lightIntensity.length - 1].time;
        float t = Mathf.Repeat(_time, duration);

        float normalized = _lightIntensity.Evaluate(t);
        _light.intensity = normalized * _maxIntensity;
    }
}