using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
public enum CameraMoveMode
{
    MoveTowards,
    Lerp
}
public class CameraPointFollow : MonoBehaviour, ISaveable
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Vector3 _offset;

    [Header("Lerping Settings")]
    [SerializeField] CameraMoveMode _moveMode = CameraMoveMode.MoveTowards;
    [SerializeField] float _lerpRate = 3f;
    [SerializeField] float _moveTowardsRate = 3f;

    [Header("Camera underwater effect")]
    [SerializeField] private AudioLowPassFilter _filter;
    [SerializeField] private AudioSource _audioSource;
    private Volume _currentUnderwaterVolume;
    private WaterZone _currentWaterZone;
    Vector3 _defaultOffset;
    private Vector3 _shakeOffset;

    private Transform _cameraFixedPoint = null;
    CameraMoveMode _defaultMoveMode;
    float _defaultLerpRate;
    float _defaultMoveTowardsRate;

    private void Awake()
    {
        _defaultOffset = _offset;
        _defaultMoveMode = CameraMoveMode.Lerp;
        _defaultLerpRate = _lerpRate;
        _defaultMoveTowardsRate = _moveTowardsRate;

        GameManager.Instance.CameraPointFollow = this;
        SaveManager.Instance.Register(this);
    }

    private void Start()
    {
        _target = GameManager.Instance.PointFollower.transform;
        transform.position = _target.position + _offset;
        _lookTarget = _target;

        if (GameManager.Instance.ActualCheckpoint != Vector3.zero)
        {
            transform.position = GameManager.Instance.ActualCheckpoint;
        }
        else
        {
            transform.position = _target.position;
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPos;

        if (_cameraFixedPoint != null)
        {
            targetPos = _cameraFixedPoint.position;
        }
        else
        {
            targetPos = _target.position + _offset + _shakeOffset;
        }

        switch (_moveMode)
        {
            case CameraMoveMode.Lerp:
                transform.position = Vector3.Lerp(
                    transform.position, targetPos, Time.deltaTime * _lerpRate);
                break;

            case CameraMoveMode.MoveTowards:
                transform.position = Vector3.MoveTowards(
                    transform.position, targetPos, Time.deltaTime * _moveTowardsRate);
                break;
        }

        if (_lookTarget != null)
        {
            Vector3 direction = _lookTarget.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Unregister(this);
    }

    public void SetUnderwaterVolume(Volume volume)
    {
        _currentUnderwaterVolume = volume;
    }

    public void SetWaterZone(WaterZone zone)
    {
        _currentWaterZone = zone;
    }

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

    public void SetOverridePosition(Transform point)
    {
        _cameraFixedPoint = point;
    }

    public void SetTargetView(Transform target)
    {
        _lookTarget = target;
    }

    public void LookAtTargetTemporary(Transform target, float timeToView)
    {
        StartCoroutine(ChangeAndReturn(target, timeToView));
    }

    IEnumerator ChangeAndReturn(Transform target, float timeToView)
    {
        _lookTarget = target;
        yield return new WaitForSeconds(timeToView);
        ResetToFollow();
        yield return null;
    }

    public void ResetToFollow()
    {
        _cameraFixedPoint = null;
        _lookTarget = _target;
        _offset = _defaultOffset;
        ResetMoveMode();
    }

    public void SetCameraOffset(Vector3 offset)
    {
        _offset = offset;
        _defaultOffset = _offset;
    }

    public void SetShakeOffset(Vector3 offset)
    {
        _shakeOffset = offset;
    }

    public void SetMoveMode(CameraMoveMode mode, float speed = -1f)
    {
        _moveMode = mode;

        if (speed > 0f)
        {
            if (mode == CameraMoveMode.Lerp) _lerpRate = speed;
            else _moveTowardsRate = speed;
        }
    }

    public void ResetMoveMode()
    {
        _moveMode = _defaultMoveMode;
        _lerpRate = _defaultLerpRate;
        _moveTowardsRate = _defaultMoveTowardsRate;
    }

    public void SetIntroPoint(Transform point, Transform lookTarget)
    {
        _cameraFixedPoint = point;
        _lookTarget = lookTarget;

        transform.position = point.position;
        transform.rotation = Quaternion.LookRotation(lookTarget.position - point.position);
    }


    public bool IsNearFollowPosition(float threshold)
    {
        Vector3 follow = _target.position + _offset;
        return (transform.position - follow).sqrMagnitude <= threshold * threshold;
    }

    public void StartResetIntro()
    {
        StartCoroutine(ResetIntro());
    }

    public void SetMoveMode(CameraMoveMode mode, float speed = -1f, bool asDefault = false)
    {
        _moveMode = mode;

        if (speed > 0f)
        {
            if (mode == CameraMoveMode.Lerp) _lerpRate = speed;
            else _moveTowardsRate = speed;
        }

        if (asDefault)
        {
            _defaultMoveMode = _moveMode;
            _defaultLerpRate = _lerpRate;
            _defaultMoveTowardsRate = _moveTowardsRate;
        }
    }

    public void ApplyPostIntroMode()
    {
        SetMoveMode(CameraMoveMode.Lerp, -1f, true);
    }

    IEnumerator ResetIntro()
    {
        _cameraFixedPoint = null;
        _offset = _defaultOffset;

        yield return new WaitForSeconds(5f);

        _lookTarget = _target;
        SetMoveMode(CameraMoveMode.Lerp, -1f, true);
    }

    public void CaptureState(SaveGameData data)
    {
        data.HasCameraState = true;
        data.CameraOffset = _defaultOffset;
    }

    public void RestoreState(SaveGameData data)
    {
        if (!data.HasCameraState) return;

        _defaultOffset = data.CameraOffset;
        _offset = _defaultOffset;
    }
}