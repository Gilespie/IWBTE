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
    [SerializeField] Color _on;
    [SerializeField] Color _off;
    [SerializeField] Light _light;
    [Header("Lever")]
    [SerializeField] private LeverMode _leverMode = LeverMode.Spring;
    [SerializeField] private LeverAxisType _axisType = LeverAxisType.Vertical;
    [SerializeField] private Transform _handle;
    [SerializeField] private Transform _handAnchor;
    [SerializeField] private float _minAngle = -45f;   // вниз
    [SerializeField] private float _maxAngle = 45f;    // вверх
    [SerializeField] private float _speed = 60f;
    [SerializeField] private float _returnSpeed = 90f;
    [SerializeField] private bool _invertAxis = false;

    [Header("Latch settings")]
    [SerializeField] private bool _startAtTop = true;            // старт в верхней позиции
    [SerializeField] private bool _lockOnMin = true;             // после опускания вниз рычаг блокируется
    [SerializeField] private bool _returnToTopIfNotFinished = true; // отпустили, не дотянув вниз, вернётся наверх

    [Header("Lever Events")]
    [SerializeField] private FloatEvent _onValueChanged; // -1 (вниз) ... 0 (центр) ... 1 (вверх)
    [SerializeField] private UnityEvent _onReachMax;
    [SerializeField] private UnityEvent _onReachMin;

    private const float CenterAngle = 0f;

    private float _currentAngle;
    private Coroutine _returnRoutine;
    private bool _atMax;
    private bool _atMin;
    private bool _isLocked;

    public Transform HandAnchor => _handAnchor;
    public bool IsLocked => _isLocked;

    private void Start()
    {
        _light.color = _off;
        _currentAngle = (_leverMode == LeverMode.Latch && _startAtTop) ? _maxAngle : CenterAngle;
        _atMax = Mathf.Approximately(_currentAngle, _maxAngle);
        ApplyAngle();
    }

    public float GetAxisValue(Vector2 rawInput)
    {
        float raw = _axisType == LeverAxisType.Vertical ? rawInput.y : rawInput.x;
        return Mathf.Clamp(raw, -1f, 1f);
    }

    // -1 (min) ... 0 (центр) ... 1 (max)
    private float GetNormalizedValue()
    {
        if (_currentAngle >= CenterAngle)
            return _maxAngle > 0f ? _currentAngle / _maxAngle : 0f;

        return _minAngle < 0f ? _currentAngle / -_minAngle : 0f;
    }

    public void SetInputValue(float value)
    {
        if (_isLocked) return;

        if (_leverMode == LeverMode.Spring)
        {
            if (Mathf.Abs(value) < 0.01f)
            {
                if (_returnRoutine == null &&
                    Mathf.Abs(_currentAngle - CenterAngle) > 0.1f)
                {
                    _returnRoutine = StartCoroutine(ReturnToAngleRoutine(CenterAngle));
                }

                return;
            }
        }
        else if (Mathf.Abs(value) < 0.01f)
        {
            return; // Latch: без ввода рычаг просто стоит на месте
        }

        StopReturn();

        // вверх (положительный input) = положительный угол
        _currentAngle = Mathf.Clamp(
            _currentAngle + value * _speed * Time.deltaTime,
            _minAngle,
            _maxAngle
        );

        ApplyAngle();
        CheckLimits();

        _onValueChanged?.Invoke(GetNormalizedValue());
    }

    public void OnRelease()
    {
        if (_isLocked) return;

        if (_leverMode == LeverMode.Spring)
        {
            StopReturn();
            _returnRoutine = StartCoroutine(ReturnToAngleRoutine(CenterAngle));
        }
        else if (_returnToTopIfNotFinished && !Mathf.Approximately(_currentAngle, _minAngle))
        {
            StopReturn();
            _returnRoutine = StartCoroutine(ReturnToAngleRoutine(_maxAngle));
        }
    }

    private void StopReturn()
    {
        if (_returnRoutine != null)
        {
            StopCoroutine(_returnRoutine);
            _returnRoutine = null;
        }
    }

    private IEnumerator ReturnToAngleRoutine(float target)
    {
        while (!Mathf.Approximately(_currentAngle, target))
        {
            _currentAngle = Mathf.MoveTowards(_currentAngle, target, _returnSpeed * Time.deltaTime);
            ApplyAngle();
            CheckLimits();
            _onValueChanged?.Invoke(GetNormalizedValue());
            yield return null;
        }

        _returnRoutine = null;
    }

    private void CheckLimits()
    {
        bool atMax = Mathf.Approximately(_currentAngle, _maxAngle);
        bool atMin = Mathf.Approximately(_currentAngle, _minAngle);

        if (atMax && !_atMax) _onReachMax?.Invoke();

        if (atMin && !_atMin)
        {
            _onReachMin?.Invoke();

            if (_leverMode == LeverMode.Latch && _lockOnMin)
            {
                _isLocked = true;
                _light.color = _on;
                StopReturn();
            }
        }

        _atMax = atMax;
        _atMin = atMin;
    }

    private void ApplyAngle()
    {
        float angle = _invertAxis ? -_currentAngle : _currentAngle;
        _handle.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}