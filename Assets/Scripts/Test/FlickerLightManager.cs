using System.Collections;
using UnityEngine;

public class FlickerLightManager : MonoBehaviour
{
    [SerializeField] Material _lampMaterial;
    [SerializeField] Color _normalColor;
    [SerializeField] Color _emergencyColor = Color.red;
    [SerializeField] AnimationCurve _intensityCurve;
    [SerializeField] float _duration = 1f;
    float _time;
    Light[] _lights;

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnExplosion, ActivateFlickering);
        EventManager.Subscribe(EventType.OnLiftFalled, SetEmergencyColor);
    }

    void Start()
    {
        _lights = GetComponentsInChildren<Light>();

        foreach (Light light in _lights)
        {
            if (light == null) continue;
            light.color = _normalColor;
        }
        
        _lampMaterial.SetColor("_EmissiveColor", _normalColor);
    }
    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnExplosion, ActivateFlickering);
        EventManager.Unsubscribe(EventType.OnLiftFalled, SetEmergencyColor);
    }

    public void ActivateFlickering(params object[] p)
    {
        StartCoroutine(FlickerLight());
    }

    public IEnumerator FlickerLight()
    {
        foreach (Light light in _lights)
        {
            if (light == null) continue;
            light.intensity = 0f;
        }

        _time = 0f;

        while (_time <= _duration)
        {
            _time += Time.deltaTime;

            float t = Mathf.Clamp01(_time / _duration);

            foreach (Light light in _lights)
            {
                if (light == null) continue;
                light.intensity = _intensityCurve.Evaluate(t);
            }

            _lampMaterial.SetFloat("_Power", _lights[0].intensity / 5f);
            yield return null;
        }
    }

    public void SetEmergencyColor(params object[] parameters)
    {
        foreach (Light light in _lights)
        {
            light.color = _emergencyColor;
        }
        _lampMaterial.SetColor("_EmissiveColor", _emergencyColor);
    }
}