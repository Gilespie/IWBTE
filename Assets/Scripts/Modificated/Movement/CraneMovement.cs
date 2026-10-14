using UnityEngine;

public class CraneMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _head;
    [SerializeField] Transform _crane;

    [Header("Rotation")]
    [SerializeField] float _rotateSpeed = 30f;
    [SerializeField] float _minYRotation = 0f;
    [SerializeField] float _maxYRotation = 180f;

    [Header("Lift")]
    [SerializeField] float _liftSpeed = 2f;         
    [SerializeField] float _minLocalY = -5f;        
    [SerializeField] float _maxLocalY = 0f;

    [Header("Smoothing")]
    [SerializeField] float _inputSmooth = 5f;
    float _rotateInput;
    float _liftInput;
    float _rotateCurrent;
    float _liftCurrent;

    float _currentYAngle;
    Quaternion _startLocalRotation;

    void Start()
    {
        _startLocalRotation = _head.localRotation;

        float startY = Mathf.DeltaAngle(0f, _head.localEulerAngles.y);
        _currentYAngle = Mathf.Clamp(startY, _minYRotation, _maxYRotation);
    }

    public void Rotate(float axis)
    {
        _rotateInput = Mathf.Clamp(axis, -1f, 1f);
    }

    public void MoveLoad(float axis)
    {
        _liftInput = Mathf.Clamp(axis, -1f, 1f);
    }

    public void StopAll()
    {
        _rotateInput = 0f;
        _liftInput = 0f;
    }

    void Update()
    {
        if (_inputSmooth > 0f)
        {
            float k = _inputSmooth * Time.deltaTime;
            _rotateCurrent = Mathf.Lerp(_rotateCurrent, _rotateInput, k);
            _liftCurrent = Mathf.Lerp(_liftCurrent, _liftInput, k);
        }
        else
        {
            _rotateCurrent = _rotateInput;
            _liftCurrent = _liftInput;
        }

        ApplyRotation();
        ApplyLift();
    }

    void ApplyRotation()
    {
        if (Mathf.Abs(_rotateCurrent) < 0.001f) return;

        _currentYAngle = Mathf.Clamp(
            _currentYAngle + _rotateCurrent * _rotateSpeed * Time.deltaTime,
            _minYRotation,
            _maxYRotation);

        _head.localRotation = Quaternion.Euler(0f, _currentYAngle, 0f);
    }

    void ApplyLift()
    {
        if (Mathf.Abs(_liftCurrent) < 0.001f) return;

        Vector3 pos = _crane.localPosition;
        pos.z = Mathf.Clamp(pos.z + _liftCurrent * _liftSpeed * Time.deltaTime, _minLocalY, _maxLocalY);
        _crane.localPosition = pos;
    }
}
