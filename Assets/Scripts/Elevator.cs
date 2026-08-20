using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private WaypointPath _waypointPath;
    private float _elapsedTime = 0f;
    private float _timeToPoint = 0f;
    private Transform _targetWaypoint;
    private Transform _previousWaypoint;
    private int _targetWaypointIndex = 0;
    private float _distanceToPoint = 0f;
    private float _elapsedPercetage = 0f;
    private Rigidbody _rb;
    private bool _isMoving = false;
    Animator _animator;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        TargetNextWaypoint();
        SetStartPosition(_targetWaypoint);
    }

    void Update()
    {
        if (!_isMoving) return;

        _elapsedTime += Time.deltaTime;

        _elapsedPercetage = _elapsedTime / _timeToPoint;

        if (_elapsedPercetage >= 1)
        {
            _isMoving = false;
            _rb.MovePosition(_targetWaypoint.position);
        }
    }

    private void FixedUpdate()
    {
        if(!_isMoving) return;

        MoveTo();
    }

    private void MoveTo()
    {
        float easedTime = Mathf.SmoothStep(0f, 1f, _elapsedPercetage);

        Vector3 newPos = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, easedTime);
        _rb.MovePosition(newPos);
    }

    public void Activate()
    {
        if (_isMoving) return;

        TargetNextWaypoint();

        _isMoving = true;
    }

    private void TargetNextWaypoint()
    { 
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;
        _elapsedPercetage = 0f;

        _distanceToPoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToPoint = _distanceToPoint / _moveSpeed;
    }

    private void SetStartPosition(Transform target)
    {
        _rb.MovePosition(target.position);
    }

    public void OnFallElevator()
    {
        _animator.enabled = true;
        this.enabled = false;
    }
}