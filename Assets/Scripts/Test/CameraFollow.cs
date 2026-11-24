using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollow : MonoBehaviour, ISaveable
{
    [Header("Camera Follow Settings")]
    [SerializeField] Transform _target;
    [SerializeField] Transform _lookTarget;
    [SerializeField] Vector3 _offset;

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
    [SerializeField] AudioLowPassFilter _filter;
    [SerializeField] AudioSource _audioSource;

    [Header("Pitch Limit (X axis)")]
    [SerializeField] bool _limitPitch = true;
    [SerializeField] float _minPitch = -10f;
    [SerializeField] float _maxPitch = 20f;

    [Header("Bob Effect")]
    [SerializeField] bool _isBobEnable = true;
    [SerializeField] float _bobSpeed = 3.5f;
    [SerializeField] float _bobIntensity = 0.05f;
    Vector3 _bobOffset;

    Volume _currentUnderwaterVolume;
    WaterZone _currentWaterZone;
    Vector3 _shakeOffset;
    Transform _cameraFixedPoint = null;

    Vector3 _defaultOffset;
    float _defaultFollowRate;

    Vector3 _targetOffset;
    Vector3 _currentOffset;
    Vector3 _offsetVelocity;

    Vector3 _currentLookPoint;
    Vector3 _lookFrom;
    float _lookBlend = 1f;
    bool _snapPending;
    float _defaultOffsetSmoothTime;

    void Awake()
    {
        _defaultOffset = _offset;
        _targetOffset = _offset;
        _currentOffset = _offset;
        _defaultFollowRate = _lerpRate;
        _defaultOffsetSmoothTime = _offsetSmoothTime;

        GameManager.Instance.CameraPointFollow = this;
        SaveManager.Instance.Register(this);
    }

    void Start()
    {
        _target = GameManager.Instance.Player.transform;
        _lookTarget = _target;

        if (GameManager.Instance.ActualCheckpoint != Vector3.zero)
            transform.position = GameManager.Instance.ActualCheckpoint;
        else
            transform.position = GetCameraPosition();

        _currentLookPoint = _lookTarget.position;
        _lookFrom = _currentLookPoint;
        _lookBlend = 1f;
    }

    void Update()
    {
        if (_isBobEnable)
        {
            BobEffect(_bobSpeed, _bobIntensity);
        }
    }

    void FixedUpdate()
    {
        if (_snapPending && _target != null)
        {
            _snapPending = false;
            SnapToTarget();
        }

        float dt = Time.deltaTime;

        _currentOffset = Vector3.SmoothDamp(
            _currentOffset, _targetOffset, ref _offsetVelocity, _offsetSmoothTime);

        Vector3 targetPos = _cameraFixedPoint != null
    ? _cameraFixedPoint.position
    : GetCameraPosition();

        float k = 1f - Mathf.Exp(-_lerpRate * dt);
        transform.position = Vector3.Lerp(transform.position, targetPos, k);

        UpdateLook(dt);
    }

    void UpdateLook(float dt)
    {
        if (_lookTarget == null) return;

        Vector3 lookPos = _lookTarget.position;

        if (_lookBlend < 1f)
        {
            _lookBlend = _lookSmoothTime > 0f
                ? Mathf.Min(1f, _lookBlend + dt / _lookSmoothTime)
                : 1f;

            float s = Mathf.SmoothStep(0f, 1f, _lookBlend);
            _currentLookPoint = Vector3.Lerp(_lookFrom, lookPos, s);
        }
        else
        {
            _currentLookPoint = lookPos;
        }

        Vector3 direction = _currentLookPoint - transform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (_limitPitch && _lookTarget == _target)
            targetRotation = ClampPitch(targetRotation);

        float rk = 1f - Mathf.Exp(-_rotationRate * dt);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rk);
    }

    Quaternion ClampPitch(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        float pitch = Mathf.DeltaAngle(0f, euler.x);
        pitch = Mathf.Clamp(pitch, _minPitch, _maxPitch);

        return Quaternion.Euler(pitch, euler.y, 0f);
    }

    void ChangeLookTarget(Transform newTarget)
    {
        if (_lookTarget == newTarget) return;

        _lookFrom = _currentLookPoint;
        _lookBlend = 0f;
        _lookTarget = newTarget;
    }

    Vector3 GetCameraPosition()
    {
        Vector3 p = _target.position + _currentOffset + _bobOffset + _shakeOffset;
        p.z = _currentOffset.z;
        return p;
    }

    void OnDestroy()
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

    IEnumerator ChangeAndReturn(Transform target, float timeToView)
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
        _offsetSmoothTime = _defaultOffsetSmoothTime;
        ResetFollowRate();
    }

    /// <summary>Плавно меняет оффсет. smoothTime &lt; 0 — оставить текущее время сглаживания.</summary>
    public void SetCameraOffset(Vector3 offset, float smoothTime = -1f)
    {
        _targetOffset = offset;

        if (smoothTime >= 0f)
            _offsetSmoothTime = smoothTime;
    }


    public void SetDefaultCameraOffset(Vector3 offset, float smoothTime = -1f)
    {
        _defaultOffset = offset;
        _targetOffset = offset;

        if (smoothTime >= 0f)
        {
            _offsetSmoothTime = smoothTime;
            _defaultOffsetSmoothTime = smoothTime;
        }
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

    public void SnapToTarget()
    {
        transform.position = GetCameraPosition();
    }

    public void SnapToFollow()
    {
        _cameraFixedPoint = null;
        _lookTarget = _target;

        transform.position = GetCameraPosition();

        _currentLookPoint = _target.position;
        _lookFrom = _currentLookPoint;
        _lookBlend = 1f;

        Vector3 dir = _currentLookPoint - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = _limitPitch ? ClampPitch(rot) : rot;
        }
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
        return (transform.position - GetCameraPosition()).sqrMagnitude <= threshold * threshold;
    }

    public void StartResetIntro()
    {
        StartCoroutine(ResetIntro());
    }

    IEnumerator ResetIntro()
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
        if (data.HasCameraState)
            SnapOffset(data.CameraOffset);

        _snapPending = true;
    }

    void BobEffect(float speed, float intensity)
    {
        float x = Mathf.Sin(Time.time * speed) * intensity;
        float y = Mathf.Cos(Time.time * speed) * intensity;

        _bobOffset = new Vector3(x, y, 0);
    }
}