using UnityEngine;

public class Lantern : MonoBehaviour
{
    [SerializeField] float _intensity = 70f;
    [SerializeField] float _radius = 40f;
    [SerializeField] Color _color;
    Light _light;
    MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material.SetColor("_EmissionColor", _color * _intensity);
        _meshRenderer.material.EnableKeyword("_EMISSION");
        _light = GetComponentInChildren<Light>();
        _light.color = _color;
        _light.intensity = _intensity;
        _light.range = _radius;
    }
}