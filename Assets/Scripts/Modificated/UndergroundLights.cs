using UnityEngine;

public class UndergroundLights : MonoBehaviour
{
    [SerializeField] GameObject[] _lights;
    [SerializeField] Material _materialLight;

    private void Awake()
    {
        TurnOnLights(false);
    }

    public void TurnOnLights(bool value)
    {
        foreach(var light in _lights)
        {
            light.SetActive(value);
        }

        if (_materialLight != null)
        {
            _materialLight.SetFloat("_EmissionStrength", value ? 1f : 0f);
        }
    }
}