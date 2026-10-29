using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraPointFollow : MonoBehaviour, ISaveable
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Vector3 _offset;

    [Header("Follow Settings")]
    [SerializeField] float _lerpRate = 3f;
    [SerializeField] float _introFollowRate = 0.8f;
    [SerializeField] float _introLookDelay = 5f;

    [Header("Offset Smoothing")]
    [SerializeField] float _offsetSmoothTime = 1f;

    [Header("Look Smoothing")]
    [SerializeField] float _rotationRate = 5f;
    [SerializeField] float _lookSmoothTime = 0.7f;

    [Header("Camera underwater effect")]
    [SerializeField] private AudioLowPassFilter _filter;
    [SerializeField] private AudioSource _audioSource;

    private Volume _currentUnderwaterVolume;
    private WaterZone _currentWaterZone;
    private Vector3 _shakeOffset;
    private Transform _cameraFixedPoint = null;

    Vector3 _defaultOffset;
    float _defaultFollowRate;

    Vector3 _targetOffset;
    Vector3 _currentOffset;
    Vector3 _offsetVelocity;

    Vector3 _currentLookPoint;
    Vector3 _lookFrom;
    float _lookBlend = 1f;

    private void Awake()
    {
        _defaultOffset = _offset;
        _targetOffset = _offset;
        _currentOffset = _offset;
        _defaultFollowRate = _lerpRate;

        GameManager.Instance.CameraPointFollow = this;
        SaveManager.Instance.Register(this);
    }

    private void Start()
    {
        _target = GameManager.Instance.PointFollower.transform;
        _lookTarget = _target;

        if (GameManager.Instance.ActualCheckpoint != Vector3.zero)
            transform.position = GameManager.Instance.ActualCheckpoint;
        else
            transform.position = _target.position + _currentOffset;

        _currentLookPoint = _lookTarget.position;
        _lookFrom = _currentLookPoint;
        _lookBlend = 1f;
    }

    private void FixedUpdate()
    {
        float dt = Time.deltaTime;

        _currentOffset = Vector3.SmoothDamp(
            _currentOffset, _targetOffset, ref _offsetVelocity, _offsetSmoothTime);

        Vector3 targetPos = _cameraFixedPoint != null
            ? _cameraFixedPoint.position
            : _target.position + _currentOffset + _shakeOffset;

        float k = 1f - Mathf.Exp(-_lerpRate * dt);
        transform.position = Vector3.Lerp(transform.position, targetPos, k);

        UpdateLook(dt);
    }

    private void UpdateLook(float dt)
    {
        if (_lookTarget == null) return;

        if (_lookBlend < 1f)
        {
            _lookBlend = _lookSmoothTime > 0f
                ? Mathf.Min(1f, _lookBlend + dt / _lookSmoothTime)
                : 1f;

            float s = Mathf.SmoothStep(0f, 1f, _lookBlend);
            _currentLookPoint = Vector3.Lerp(_lookFrom, _lookTarget.position, s);
        }
        else
        {
            _currentLookPoint = _lookTarget.position;
        }

        Vector3 direction = _currentLookPoint - transform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rk = 1f - Mathf.Exp(-_rotationRate * dt);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rk);
    }

    private void ChangeLookTarget(Transform newTarget)
    {
        if (_lookTarget == newTarget) return;

        _lookFrom = _currentLookPoint;
        _lookBlend = 0f;
        _lookTarget = newTarget;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Unregister(this);
    }

    public void SetUnderwaterVolume(Volume volume) => _currentUnderwaterVolume = volume;
    public void SetWaterZone(WaterZone zone) => _currentWaterZone = zone;

    public void EnterWater()
    {
        if (_currentUnderwaterVolume != null)
            _currentUnderwaterVolume.enabled = true;

        _filter.enabled = true;
        _audioSource.enabled = true;
    }

    public void ExitWater()
    {
        if (_currentUnderwaterVolume != null)
            _currentUnderwaterVolume.enabled = false;

        _currentUnderwaterVolume = null;

        _filter.enabled = false;
        _audioSource.enabled = false;
    }

    public void SetOverridePosition(Transform point) => _cameraFixedPoint = point;

    public void SetTargetView(Transform target) => ChangeLookTarget(target);

    public void LookAtTargetTemporary(Transform target, float timeToView)
    {
        StartCoroutine(ChangeAndReturn(target, timeToView));
    }

    private IEnumerator ChangeAndReturn(Transform target, float timeToView)
    {
        ChangeLookTarget(target);
        yield return new WaitForSeconds(timeToView);
        ResetToFollow();
    }

    public void ResetToFollow()
    {
        _cameraFixedPoint = null;
        ChangeLookTarget(_target);
        _targetOffset = _defaultOffset;
        ResetFollowRate();
    }

    /// <summary>Плавно меняет оффсет. smoothTime &lt; 0 — оставить текущее время сглаживания.</summary>
    public void SetCameraOffset(Vector3 offset, float smoothTime = -1f)
    {
        _targetOffset = offset;
        _defaultOffset = offset;

        if (smoothTime >= 0f)
            _offsetSmoothTime = smoothTime;
    }

    /// <summary>Мгновенно ставит оффсет (для загрузки/респавна).</summary>
    public void SnapOffset(Vector3 offset)
    {
        _offset = offset;
        _targetOffset = offset;
        _currentOffset = offset;
        _defaultOffset = offset;
        _offsetVelocity = Vector3.zero;
    }

    public void SnapToFollow()
    {
        _cameraFixedPoint = null;
        _lookTarget = _target;

        transform.position = _target.position + _currentOffset;

        _currentLookPoint = _target.position;
        _lookFrom = _currentLookPoint;
        _lookBlend = 1f;

        Vector3 dir = _currentLookPoint - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetShakeOffset(Vector3 offset) => _shakeOffset = offset;


    public void SetFollowRate(float rate, bool asDefault = false)
    {
        if (rate <= 0f) return;

        _lerpRate = rate;

        if (asDefault)
            _defaultFollowRate = rate;
    }

    public void ResetFollowRate() => _lerpRate = _defaultFollowRate;


    public void SetIntroPoint(Transform point, Transform lookTarget)
    {
        _cameraFixedPoint = point;
        _lookTarget = lookTarget;

        transform.position = point.position;
        transform.rotation = Quaternion.LookRotation(lookTarget.position - point.position);

        _currentLookPoint = lookTarget.position;
        _lookFrom = _currentLookPoint;
        _lookBlend = 1f;
    }

    public bool IsNearFollowPosition(float threshold)
    {
        Vector3 follow = _target.position + _currentOffset;
        return (transform.position - follow).sqrMagnitude <= threshold * threshold;
    }

    public void StartResetIntro()
    {
        StartCoroutine(ResetIntro());
    }

    private IEnumerator ResetIntro()
    {
        _cameraFixedPoint = null;
        _targetOffset = _defaultOffset;
        SetFollowRate(_introFollowRate);

        yield return new WaitForSeconds(_introLookDelay);

        ChangeLookTarget(_target);
        ResetFollowRate();
    }

    public void CaptureState(SaveGameData data)
    {
        data.HasCameraState = true;
        data.CameraOffset = _defaultOffset;
    }

    public void RestoreState(SaveGameData data)
    {
        if (!data.HasCameraState) return;

        SnapOffset(data.CameraOffset);
    }
}