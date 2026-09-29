using UnityEngine;

public class ConstrucBuilding : MonoBehaviour
{
    [SerializeField] Light[] _lights;
    [SerializeField] Material _lampMat;

    private void Awake()
    {
        _lampMat.SetColor("_EmissionColor", Color.black);
        TurnOnLights(false);
    }

    public void TurnOnLights(bool value)
    {
        foreach (var light in _lights)
        {
            light.enabled = value;
        }
    }
}