using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraPointFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private bool _isStaticZ = false;

    [Header("Lerping Settings")]
    [SerializeField] bool _lerp = false;
    [SerializeField] float _lerpRate = 3f;
    [SerializeField] float _moveTowardsRate = 3f;

    [Header("Camera underwater effect")]
    [SerializeField] private AudioLowPassFilter _filter;
    [SerializeField] private AudioSource _audioSource;
    private Volume _currentUnderwaterVolume;
    private WaterZone _currentWaterZone;
    float _defaultZPos;
    Vector3 _defaultOffset;
    private Vector3 _shakeOffset;
    float _zPos = -8f;

    private Transform _cameraFixedPoint = null;

    private void Awake()
    {
        _defaultZPos = _offset.z;
        _defaultOffset = _offset;
        _lerp = true;
        GameManager.Instance.CameraPointFollow = this;
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

        if(_lerp)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * _lerpRate
            );
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                Time.deltaTime * _moveTowardsRate
            );
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
        Debug.Log("Entered water");
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
        _offset.z = _defaultZPos;
        ActiveLerpSmoothing(true);
    }

    public void SetZPos(float value)
    {
        _zPos = value;
        _offset.z = _zPos;
    }

    public void SetCameraOffset(Vector3 offset)
    {
        _offset = offset;
    }

    public void SetShakeOffset(Vector3 offset)
    {
        _shakeOffset = offset;
    }

    public void ActiveLerpSmoothing(bool value)
    {
        _lerp = value;
    }
}