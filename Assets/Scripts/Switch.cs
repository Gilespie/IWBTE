using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum LeverAxisType
{
    Vertical,  
    Horizontal  
}

public enum LeverMode
{
    Spring, 
    Latch
}

[System.Serializable] public class FloatEvent : UnityEvent<float> { }

public class Switch : MonoBehaviour, IControllable
{
    /*[SerializeField] private Color _colorOn;
    [SerializeField] private Color _colorOff;
    [SerializeField] private AudioClip _sfxOn;
    [SerializeField] private AudioClip _sfxOff;
    [SerializeField] private Light _light;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private MeshRenderer _meshRenderer;*/

    [Header("Lever")]
    [SerializeField] private LeverMode _leverMode = LeverMode.Spring;
    [SerializeField] private LeverAxisType _axisType = LeverAxisType.Vertical;
    [SerializeField] private Transform _handle;
    [SerializeField] private Transform _handAnchor;
    [SerializeField] private float _minAngle = 0f;
    [SerializeField] private float _centerAngle = 40f;
    [SerializeField] private float _maxAngle = 80f;
    [SerializeField] private float _speed = 60f;
    [SerializeField] private float _returnSpeed = 90f; 
    [SerializeField] private bool _invertAxis = false;

    [Header("Lever Events")]
    [SerializeField] private FloatEvent _onValueChanged; 
    [SerializeField] private UnityEvent _onReachMax;
    [SerializeField] private UnityEvent _onReachMin;

    private float _currentAngle;
    private Coroutine _returnRoutine;

    public Transform HandAnchor => _handAnchor;

    private void Start()
    {
        _currentAngle = _centerAngle;
        ApplyAngle();
    }

    public float GetAxisValue(Vector2 rawInput)
    {
        float raw = _axisType == LeverAxisType.Vertical ? rawInput.y : rawInput.x;
        return Mathf.Clamp(raw, -1f, 1f);
    }
   
    private float GetNormalizedValue()
    {
        if (_currentAngle >= _centerAngle)
        {
            float halfRangeDown = _maxAngle - _centerAngle;
            return halfRangeDown > 0f ? (_currentAngle - _centerAngle) / halfRangeDown : 0f;
        }
        else
        {
            float halfRangeUp = _centerAngle - _minAngle;
            return halfRangeUp > 0f ? (_currentAngle - _centerAngle) / halfRangeUp : 0f;
        }
    }

    public void SetInputValue(float value)
    {
        if (_leverMode != LeverMode.Spring)
            return;

        if (Mathf.Abs(value) < 0.01f)
        {
            if (_returnRoutine == null &&
                Mathf.Abs(_currentAngle - _centerAngle) > 0.1f)
            {
                _returnRoutine = StartCoroutine(ReturnToCenterRoutine());
            }

            return;
        }

        if (_returnRoutine != null)
        {
            StopCoroutine(_returnRoutine);
            _returnRoutine = null;
        }

        _currentAngle = Mathf.Clamp(
            _currentAngle - value * _speed * Time.deltaTime,
            _minAngle,
            _maxAngle
        );

        ApplyAngle();

        _onValueChanged?.Invoke(-GetNormalizedValue());
    }

    public void OnRelease()
    {
        if (_leverMode != LeverMode.Spring) return;

        if (_returnRoutine != null) StopCoroutine(_returnRoutine);
        _returnRoutine = StartCoroutine(ReturnToCenterRoutine());
    }

    private IEnumerator ReturnToCenterRoutine()
    {
        while (Mathf.Abs(_currentAngle - _centerAngle) > 0.1f)
        {
            _currentAngle = Mathf.MoveTowards(_currentAngle, _centerAngle, _returnSpeed * Time.deltaTime);
            ApplyAngle();
            _onValueChanged?.Invoke(-GetNormalizedValue());
            yield return null;
        }

        _currentAngle = _centerAngle;
        ApplyAngle();
        _onValueChanged?.Invoke(0f);
        _returnRoutine = null;
    }

    private void ApplyAngle()
    {
        _handle.localRotation = Quaternion.Euler(_currentAngle, 0f, 0f);
    }
}