using System.Collections;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("Bob Effect")]
    [SerializeField] private bool _isBobEnable = true;
    [SerializeField] private float _bobSpeed = 3.5f;
    [SerializeField] private float _bobIntensity = 0.05f;

    [SerializeField] private float _speedRate = 5f;
    [SerializeField] private float _targetFov = 75f;
    [SerializeField] private float _transitionTime = 1f;
    [SerializeField] private float _delayTime = 8f;
    [SerializeField] private float _zPosConstant = -10f;
    [SerializeField] private Vector3 _offset;

    [Header("Point of view")]
    [SerializeField] private Vector3 _airplaneCrashOffset;
    [SerializeField] private Vector3 _airplaneCrash2Offset;

    private Camera _camera;
    private Transform _target;
    private Vector3 _currentPosition;
    private Vector3 _shakeOffset;
    private Vector3 _bobOffset;
    private Vector3 _defaultOffset;
    private float _defaultFOV = 60f;
    private float _currentFOV;

    private void Awake()
    {
        //GameManager.Instance.Camera = this;    
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.fieldOfView = _defaultFOV;

        _defaultOffset = _offset;

        _target = GameManager.Instance.Player.transform;

        if (GameManager.Instance.ActualCheckpoint != Vector3.zero)
        {
            transform.position = GameManager.Instance.ActualCheckpoint;
        }
        else
        {
            transform.position = _target.position;
        }
    }

    private void Update()
    {
        if (_isBobEnable)
        {
            BobEffect(_bobSpeed,_bobIntensity);
        }
    }

    private void FixedUpdate()
    {
        _currentPosition = _target.position + _offset + _bobOffset + _shakeOffset;

        _currentPosition.z = _zPosConstant;

        transform.position = Vector3.Lerp(transform.position, _currentPosition, _speedRate * Time.fixedDeltaTime);
        transform.LookAt(_target);
    }

    public void SetShakeOffset(Vector3 offset)
    {
        _shakeOffset = offset;
    }

    public void ActiveAirplaneOffset()
    {
        StartCoroutine(SmoothOffsetRoutine(_airplaneCrashOffset));
    }

    public void ActiveAirplane2Offset()
    {
        StartCoroutine(SmoothOffsetRoutine(_airplaneCrash2Offset));
    }

    public void ActiveBaseOffset()
    {
        StartCoroutine(SmoothOffsetRoutine(_defaultOffset));
    }

    public void ActiveFOVRoutine()
    {
        StartCoroutine(FOVRoutine(_targetFov));
    }

    public void ActiveDefaultFOVRoutine()
    {
        StartCoroutine(FOVRoutine(_defaultFOV));
    }

    private void BobEffect(float speed, float intensity)
    {
        float x = Mathf.Sin(Time.time * speed) * intensity;
        float y = Mathf.Cos(Time.time * speed) * intensity;

        _bobOffset = new Vector3(x, y, 0);
    }

    private IEnumerator SmoothOffsetChange(Vector3 newOffset)
    {
        _defaultOffset = _offset;

        Vector3 startOffset = _offset;
        
        float elapsed = 0f;

        while (elapsed < _transitionTime)
        {
            _offset = Vector3.Lerp(startOffset, newOffset, elapsed / _transitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _offset = newOffset;

        yield return new WaitForSeconds(_delayTime);

        elapsed = 0f;

        while (elapsed < _transitionTime)
        {
            _offset = Vector3.Lerp(newOffset, _defaultOffset, elapsed / _transitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _offset = _defaultOffset;

        yield return null;
    }

    private IEnumerator SmoothOffsetRoutine(Vector3 newOffset)
    {
        Vector3 startOffset = _offset;

        float elapsed = 0f;

        while (elapsed < _transitionTime)
        {
            _offset = Vector3.Lerp(startOffset, newOffset, elapsed / _transitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _offset = newOffset;

        yield return null;
    }

    private IEnumerator FOVRoutine(float newFOV)
    {
        
        float startFOV = _camera.fieldOfView;

        float elapsed = 0f;

        while (elapsed < _transitionTime)
        {
            _currentFOV = Mathf.Lerp(startFOV, newFOV, elapsed / _transitionTime);
            _camera.fieldOfView = _currentFOV;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _currentFOV = newFOV;

        yield return null;
    }
}