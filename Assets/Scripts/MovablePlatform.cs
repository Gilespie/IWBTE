using System.Collections;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    [SerializeField] private WaypointPath _waypointPath;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _offset = 0.1f;
    [SerializeField] private float _waitSeconds = 3f;
    private Transform _targetWaypoint;
    private int _targetWaypointIndex = 0;
    private float _distanceToWaypoint = 0f;
    private bool _isMoving = true;
    private Rigidbody _rb;
    private Coroutine _currentRoutine = null;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        TargetNextWaypoint();
    }

    void Update()
    {
        if (GetDistance() <= _offset)
        {
            if (_currentRoutine == null)
            {
                _currentRoutine = StartCoroutine(AwaitingRoutine());
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            MoveTo();
        }
    }

    private float GetDistance()
    {
        return _distanceToWaypoint = Vector3.Distance(transform.position, _targetWaypoint.position);
    }

    private void MoveTo()
    {
        Vector3 dir = (_targetWaypoint.position - transform.position).normalized;
        _rb.MovePosition(transform.position + dir * _speed * Time.fixedDeltaTime);
    }

    private void TargetNextWaypoint()
    {
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
    }

    private IEnumerator AwaitingRoutine()
    {
        _isMoving = false;
        yield return new WaitForSeconds(_waitSeconds);
        TargetNextWaypoint();
        _isMoving = true;

        _currentRoutine = null;

        yield return null;
    }
}