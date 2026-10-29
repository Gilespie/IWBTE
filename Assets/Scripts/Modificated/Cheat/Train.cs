using UnityEngine;
using UnityEngine.Events;

public class Train : MonoBehaviour
{
    enum TrainState
    {
        Idle,
        Accelerating,
        Cruising,
        Braking
    }

    [Header("References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _trainSound;

    [Header("Stations")]
    [SerializeField] Transform[] _stations;
    [SerializeField] int _currentStationIndex = 0;
    [SerializeField] bool _loop = false;       
    [SerializeField] bool _snapToStartStation = true;

    [Header("Movement")]
    [SerializeField] float _acceleration = 5f;
    [SerializeField] float _brakeForce = 5f;
    [SerializeField] float _maxLinearSpeed = 10f;
    [SerializeField] float _minBrakeSpeed = 0.3f;
    [SerializeField] float _stopDistance = 0.02f;

    [Header("Events")]
    [SerializeField] UnityEvent _onTripStarted;
    [SerializeField] UnityEvent _onArrived;
    [SerializeField] UnityEvent _onLastStationReached;

    TrainState _state = TrainState.Idle;
    float _currentSpeed;
    int _targetStationIndex;
    Vector3 _direction = Vector3.right;

    public bool IsMoving => _state != TrainState.Idle;
    public int CurrentStationIndex => _currentStationIndex;
    public Vector3 ExternalVelocity => _direction * _currentSpeed;

    void Start()
    {
        if (_snapToStartStation && _stations != null && _stations.Length > 0)
        {
            _currentStationIndex = Mathf.Clamp(_currentStationIndex, 0, _stations.Length - 1);
            _rb.position = _stations[_currentStationIndex].position;
        }
    }

    void FixedUpdate()
    {
        if (_state != TrainState.Idle)
            Drive();

        ChangePitch();
    }

    void Drive()
    {
        Vector3 toTarget = _stations[_targetStationIndex].position - _rb.position;
        float remaining = toTarget.magnitude;

        if (remaining > 0.0001f)
            _direction = toTarget / remaining;

        float dt = Time.fixedDeltaTime;

        float brakingDistance = (_currentSpeed * _currentSpeed) / (2f * _brakeForce);

        if (_state != TrainState.Braking && remaining <= brakingDistance)
            _state = TrainState.Braking;

        switch (_state)
        {
            case TrainState.Accelerating:
                _currentSpeed += _acceleration * dt;

                if (_currentSpeed >= _maxLinearSpeed)
                {
                    _currentSpeed = _maxLinearSpeed;
                    _state = TrainState.Cruising;
                }
                break;

            case TrainState.Cruising:
                break;

            case TrainState.Braking:
                float desired = Mathf.Sqrt(2f * _brakeForce * remaining);
                desired = Mathf.Max(desired, _minBrakeSpeed);
                _currentSpeed = Mathf.Min(_currentSpeed, desired);
                break;
        }

        float step = _currentSpeed * dt;

        if (remaining <= _stopDistance || step >= remaining)
        {
            Arrive();
            return;
        }

        _rb.MovePosition(_rb.position + _direction * step);
    }

    void Arrive()
    {
        _rb.MovePosition(_stations[_targetStationIndex].position);

        _currentSpeed = 0f;
        _currentStationIndex = _targetStationIndex;
        _state = TrainState.Idle;

        _onArrived?.Invoke();

        if (!_loop && _currentStationIndex == _stations.Length - 1)
            _onLastStationReached?.Invoke();
    }

    /// <summary>UnityEvent -  Send train to the next station.</summary>
    public void StartTrip()
    {
        if (_state != TrainState.Idle) return;
        if (_stations == null || _stations.Length < 2) return;

        int next = _currentStationIndex + 1;

        if (next >= _stations.Length)
        {
            if (!_loop) return;
            next = 0;
        }

        _targetStationIndex = next;
        _state = TrainState.Accelerating;
        _onTripStarted?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
            character.transform.SetParent(transform);
    }

    void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
            character.transform.SetParent(null);
    }

    void ChangePitch()
    {
        float normalizedSpeed = _currentSpeed / _maxLinearSpeed;
        _audioSource.pitch = Mathf.Lerp(1f, 2f, normalizedSpeed);
    }

    void OnDrawGizmos()
    {
        if (_stations == null) return;

        for (int i = 0; i < _stations.Length; i++)
        {
            if (_stations[i] == null) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_stations[i].position, 0.5f);

            if (i < _stations.Length - 1 && _stations[i + 1] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_stations[i].position, _stations[i + 1].position);
            }
        }
    }
}