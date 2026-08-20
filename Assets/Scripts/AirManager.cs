using UnityEngine;
using UnityEngine.Rendering;

public class AirManager : MonoBehaviour
{
    [SerializeField] private float _maxOxygen = 10f;
    [SerializeField] private Collider _headCollider;
    [SerializeField] Material _oxigentFullscreen;
    private bool _isUnderwater = false;
    private float _currentOxygen = 0;
    public float CurrentOxygen => _currentOxygen;
    private WaterZone _currentWaterZone;

    private void Start()
    {
        _currentOxygen = _maxOxygen;
        //_oxigentFullscreen.SetFloat("_Intensity", 1 -  _currentOxygen / _maxOxygen);
    }

    private void Update()
    {
        if (_currentWaterZone == null) return;

        bool headUnderwater = _headCollider.bounds.center.y < _currentWaterZone.BoundY;

        if (headUnderwater && !_isUnderwater)
        {
            EnterWater();
        }
        else if (!headUnderwater && _isUnderwater)
        {
            ExitWater();
        }

        if (_isUnderwater)
        {
            _currentOxygen -= Time.deltaTime;

            //_oxigentFullscreen.SetFloat("_Intensity", 1 - _currentOxygen / _maxOxygen);

            if (_currentOxygen <= 0)
            {
                EventManager.Trigger(EventType.OnFinishOxygen);
            }
        }
        else
        {
            //_oxigentFullscreen.SetFloat("_Intensity", 1 - _currentOxygen / _maxOxygen);
        }
    }

    public void SetWaterZone(WaterZone zone)
    {
        _currentWaterZone = zone;
    }

    public void EnterWater()
    {
        _isUnderwater = true;
    }

    public void ExitWater()
    {
        _isUnderwater = false;
        _currentOxygen = _maxOxygen;
    }
}