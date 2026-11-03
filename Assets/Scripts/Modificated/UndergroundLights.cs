using UnityEngine;

public class UndergroundLights : MonoBehaviour
{
    [SerializeField] GameObject[] _lights;
    [SerializeField] Material _materialLight;

    [Header("Emission")]
    [SerializeField, ColorUsage(false, true)] Color _emissionColor = Color.white; 
    [SerializeField] float _intensityOn = 3f;
    [SerializeField] float _intensityOff = 0f;
    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        TurnOnLights(false);
    }

    public void TurnOnLights(bool value)
    {
        foreach (var light in _lights)
        {
            light.SetActive(value);
        }

        if (_materialLight != null)
        {
            SetEmission(value ? _intensityOn : _intensityOff);
        }
    }

    void SetEmission(float intensity)
    {
        _materialLight.EnableKeyword("_EMISSION");
        _materialLight.SetColor(EmissionColorId, _emissionColor * intensity);
    }
}