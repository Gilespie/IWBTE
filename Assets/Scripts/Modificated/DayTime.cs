using UnityEngine;
using System;

[ExecuteAlways]
public class DayTime : MonoBehaviour
{
    [SerializeField] private Light _dirLight;
    [SerializeField] private Gradient _dayGradient;
    [SerializeField] private Gradient _ambientColor;
    [SerializeField, Range(0f, 1f)] private float _timeProgress = 0;
    [SerializeField, Range(0f, 3600f)] private float _timeInSeconds = 60;
    [SerializeField] private bool _isRun = false;
    private Vector3 _defaultAngle;

    private void Start()
    {
        _defaultAngle = _dirLight.transform.localEulerAngles;
    }

    private void Update()
    {
        _dirLight.color = _dayGradient.Evaluate(_timeProgress);
        RenderSettings.ambientLight = _ambientColor.Evaluate(_timeProgress);

        if (_isRun)
        {
            _timeProgress += Time.deltaTime / _timeInSeconds;

            if (_timeProgress > 1f)
            {
                _timeProgress = 0f;
            }

            _dirLight.color = _dayGradient.Evaluate(_timeProgress);
            RenderSettings.ambientLight = _ambientColor.Evaluate(_timeProgress);

            _dirLight.transform.localEulerAngles = new Vector3(360f * -_timeProgress - 90f, _defaultAngle.y, _defaultAngle.z);
        }
    }
}