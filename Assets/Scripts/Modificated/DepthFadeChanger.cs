using UnityEngine;

public class DepthFadeChanger : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Color _colorDay;
    [SerializeField] Color _colorNight;
    [SerializeField] bool _isNight;

    private void Awake()
    {
        ChangeColorFade(_isNight);
    }

    private void OnDisable()
    {
        _mat.color = _colorDay;
    }

    private void OnApplicationQuit()
    {
        _mat.color = _colorDay;
    }

    void ChangeColorFade(bool isNight)
    {
        _isNight = isNight;
        _mat.color = _isNight ? _colorNight : _colorDay;
    }
}