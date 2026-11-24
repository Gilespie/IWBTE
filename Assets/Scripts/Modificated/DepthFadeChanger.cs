using UnityEngine;

public class DepthFadeChanger : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Color _colorDay;
    [SerializeField] Color _colorNight;
    [SerializeField] bool _isNight;

    [SerializeField] float _distanceDay = 0.5f;
    [SerializeField] float _distanceNight = 0.5f;
    static readonly int MaxDistanceID =
        Shader.PropertyToID("_MaxDistance");

    void Awake()
    {
        ChangeColorFade(_isNight);
    }

    void OnDisable()
    {
        _mat.color = _colorDay;
        _mat.SetFloat(MaxDistanceID, _distanceDay);
    }

    void OnApplicationQuit()
    {
        _mat.color = _colorDay;
        _mat.SetFloat(MaxDistanceID, _distanceDay);
    }

    public void ChangeColorFade(bool isNight)
    {
        _isNight = isNight;

        _mat.color = _isNight ? _colorNight : _colorDay;

        _mat.SetFloat(
            MaxDistanceID,
            _isNight ? _distanceNight : _distanceDay
        );
    }

    public void SetMaxDistance(float distance)
    {
        _mat.SetFloat(MaxDistanceID, distance);
    }
}