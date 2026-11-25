using UnityEngine;

public class DepthFadeChanger : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Color _colorDay;
    [SerializeField] Color _colorNight;
    [SerializeField] bool _isNight;

    static readonly int MaxDistanceID =
        Shader.PropertyToID("_MaxDistance");

    void Awake()
    {
        ChangeColorFade(_isNight);
    }

    void OnDisable()
    {
        _mat.color = _colorDay;
    }

    void OnApplicationQuit()
    {
        _mat.color = _colorDay;
    }

    public void ChangeColorFade(bool isNight)
    {
        _isNight = isNight;

        _mat.color = _isNight ? _colorNight : _colorDay;
    }

    public void SetMaxDistance(float distance)
    {
        _mat.SetFloat(MaxDistanceID, distance);
    }
}