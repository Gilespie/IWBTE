using UnityEngine;


public class DronMovement : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] bool _isCutsceneDrone = false;
    [SerializeField] Transform _targetPoint;

    [Header("Movement")]
    [SerializeField] private float _patrolSpeed = 5f;
    [SerializeField] private float _chaseSpeed = 8f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float targetChangeInterval = 2f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _maxSpeedLimit = 5f;
    [SerializeField] private float _arrivalDistance = 1f;
    Vector3 _currentPosTarget;

    [Header("AI")]
    [SerializeField] private DroneStates _droneStates;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _eyePoint;

    [Header("FOV")]
    [SerializeField] private LayerMask _visionMask;
    private float _fovDistance;
    private float _fovAngle;

    [Header("AI - Light")]
    [SerializeField] private Light _droneLight;
    [SerializeField] private Color _patrolColor = Color.cyan;
    [SerializeField] private Color _chaseColor = Color.red;
    [SerializeField] private Color _searchColor = Color.yellow;
    [SerializeField] private bool _startIdle = true;

    [Header("Explosion")]
    [SerializeField] float _explosionForce = 3000;
    [SerializeField] float _explosionUpForce = 1;
    [SerializeField] float _explosionRadius = 5;
    [SerializeField] float _explodeDistance = 3;
    [SerializeField] LayerMask _damageMask;
    [SerializeField] GameObject _explosiveVFX;

    [Header("SFX")]
    [SerializeField] AudioClip _detectClip;
    [SerializeField] AudioSource _detectAudioSource;
    [SerializeField] private float _minBeepInterval = 0.1f;
    [SerializeField] private float _maxBeepInterval = 2f;
    [SerializeField] private float _maxBeepDistance = 15f;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;
    int _currentIndex = 0;  
    private float _beepTimer;
    Vector3 _currentTarget;
    Vector3 _lastSeenPosition;

    bool _isExploded = false;
    DroneStates _state = DroneStates.Patrol;

    private void Start()
    {
        _fovAngle = _droneLight.spotAngle;
        _fovDistance = _droneLight.range;

        if (_startIdle)
        {
            _state = DroneStates.Idle;
        }
        else
        {
            if (_patrolPoints != null && _patrolPoints.Length > 0)
            {
                _currentIndex = Random.Range(0, _patrolPoints.Length);
                _currentTarget = _patrolPoints[_currentIndex].position;
            }

            _state = DroneStates.Patrol;
        }

        SetLightColor(_patrolColor);
    }

    private void FixedUpdate()
    {
        if (!_isCutsceneDrone)
        {
            UpdateState();
            MoveToTarget();
            CheckExplosion();
            HandleBeepSound();
        }
        else
        {
            CutsceneDron();
        }
    }

    void UpdateState()
    {
        bool sees = CanSeePlayer();

        switch (_state)
        {
            case DroneStates.Idle:
                if (sees)
                {
                    EnterChase();
                }
                break;

            case DroneStates.Patrol:
                if (sees) EnterChase();
                break;

            case DroneStates.Chase:
                if (sees)
                {
                    _lastSeenPosition = _player.position;
                    _currentTarget = _player.position;
                }
                else
                {
                    EnterInvestigate();
                }
                break;

            case DroneStates.Investigate:
                if (sees)
                {
                    EnterChase();
                }
                else if ((transform.position - _lastSeenPosition).sqrMagnitude < _arrivalDistance * _arrivalDistance)
                {
                    if (_startIdle)
                        EnterIdle();
                    else
                        EnterPatrol();
                }
                else
                {
                    _currentTarget = _lastSeenPosition;
                }
                break;

            case DroneStates.Cutscene:
                EnterChase();
                _currentTarget = _targetPoint.position;
                break;
        }
    }

    void EnterChase()
    {
        _state = DroneStates.Chase;
        SetLightColor(_chaseColor);
    }

    void EnterInvestigate()
    {
        _state = DroneStates.Investigate;
        _currentTarget = _lastSeenPosition;
        SetLightColor(_searchColor);
    }

    void EnterPatrol()
    {
        _state = DroneStates.Patrol;
        _lastSeenPosition = Vector3.zero;
        _currentTarget = GetNextTarget();
        SetLightColor(_patrolColor);
    }

    void EnterIdle()
    {
        _state = DroneStates.Idle;
        _rb.linearVelocity = Vector3.zero;
        SetLightColor(_patrolColor);
    }

    bool CanSeePlayer()
    {
        float sqrtDistance = (_player.position - _eyePoint.position).sqrMagnitude;

        if (sqrtDistance < _fovDistance * _fovDistance)
        {
            Vector3 dirToPlayer = (_player.position - _eyePoint.position).normalized;
            float angleBetweenDronAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenDronAndPlayer < _fovAngle * 0.5f)
            {
                if (!Physics.Linecast(_eyePoint.position, _player.position, _visionMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private Vector3 GetNextTarget()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return transform.position;
        }

        _currentIndex = Random.Range(0, _patrolPoints.Length);

        return _patrolPoints[_currentIndex].position;
    }

    void CheckExplosion()
    {
        if (_isExploded) return;

        if (!CanSeePlayer())
            return;

        float sqrDistance = (_player.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _explodeDistance * _explodeDistance)
        {
            Explode();
        }
    }

    void MoveToTarget()
    {
        if (_state == DroneStates.Idle)
            return;

        Vector3 direction = (_currentTarget - transform.position).normalized;

        Vector3 flatDirection = _currentTarget - transform.position;
        flatDirection.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        if (_rb.linearVelocity.magnitude < _maxSpeedLimit)
        {
            _rb.AddForce(direction * _patrolSpeed, ForceMode.Force);
        }

        if (_state == DroneStates.Patrol &&
            (_currentTarget - transform.position).sqrMagnitude < _arrivalDistance * _arrivalDistance)
        {
            _currentTarget = GetNextTarget();
        }
    }

    void Explode()
    {
        if (_isExploded) return;
        _isExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius, _damageMask);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.InstantKill();

                Rigidbody[] rbs = hit.GetComponentsInChildren<Rigidbody>();

                if (rbs != null)
                {
                    foreach (Rigidbody rb in rbs)
                    {
                        rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _explosionUpForce);
                    }
                }
            }
        }

        SpawnVFX();
        Destroy(gameObject);
    }

    void SetLightColor(Color color)
    {
        if (_droneLight != null)
            _droneLight.color = color;
    }

    private void SpawnVFX()
    {
        Instantiate(_explosiveVFX, transform.position, Quaternion.identity);
    }

    void CutsceneDron()
    {
        _state = DroneStates.Chase;
        SetLightColor(_chaseColor);
        Vector3 dir = (_targetPoint.position - transform.position);
        float distanceSQRT = dir.sqrMagnitude;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        _rb.MovePosition(_rb.position + dir.normalized * _patrolSpeed * Time.fixedDeltaTime);

        if(distanceSQRT < _arrivalDistance * _arrivalDistance)
        {
            Explode();
        }

        HandleBeepSound();
    }
    
    void HandleBeepSound()
    {
        if (_state != DroneStates.Chase) return;

        float distance = Vector3.Distance(transform.position, _player.position);
        float t = Mathf.Clamp01(distance / _maxBeepDistance);
        float interval = Mathf.Lerp(_minBeepInterval, _maxBeepInterval, t);

        _beepTimer -= Time.deltaTime;

        if (_beepTimer <= 0f)
        {
            _detectAudioSource.PlayOneShot(_detectClip);
            _beepTimer = interval;
        }
    }

    private void OnDrawGizmos()
    {
        //fov
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _fovDistance);

        // радиус взрыва
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}