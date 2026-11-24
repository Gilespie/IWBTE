using UnityEngine;

public class RailroadTrafficLight : MonoBehaviour
{
    [SerializeField, ColorUsage(false, true)] Color _startColor;
    [SerializeField, ColorUsage(false, true)] Color _stopColor;
    [SerializeField] Material _mat;
    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        _mat.SetColor(EmissionColorId, _startColor);
    }

    public void ChangeColor()
    {
        _mat.SetColor(EmissionColorId, _stopColor);
    }
}