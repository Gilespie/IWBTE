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
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _arrivalDistance = 1f;
    [SerializeField] private float _acceleration = 10f;

    [Header("AI")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _eyePoint;

    [Header("FOV")]
    [SerializeField] private LayerMask _obstacleMask;
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
    [SerializeField] bool _randomPatrol = true;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask _obstacleAvoidanceMask;
    [SerializeField] private float _avoidanceRayLength = 3f;     
    [SerializeField] private float _avoidanceSideOffset = 0.5f;  
    [SerializeField] private float _avoidanceStrength = 2f;      
    [SerializeField] private int _avoidanceRays = 3;             

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
            if (_randomPatrol)
                _currentIndex = Random.Range(0, _patrolPoints.Length);
            else
                _currentIndex = 0;

            _currentTarget = _patrolPoints[_currentIndex].position;

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
        Vector3 playerTarget = _player.position + Vector3.up;
        Vector3 eyePos = _eyePoint.position;

        float sqrDistance = (playerTarget - eyePos).sqrMagnitude;
        if (sqrDistance > _fovDistance * _fovDistance)
            return false;

        Vector3 dirToPlayer = (playerTarget - eyePos).normalized;
        Vector3 eyeForward = _droneLight.transform.forward;

        float angle = Vector3.Angle(eyeForward, dirToPlayer);
        if (angle > _fovAngle * 0.5f)
            return false;

        if (Physics.Linecast(eyePos, playerTarget, _obstacleMask))
            return false;

        return true;
    }

    private Vector3 GetNextTarget()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return transform.position;
        }

        if(_randomPatrol)
            _currentIndex = Random.Range(0, _patrolPoints.Length);
        else
            _currentIndex = (_currentIndex + 1) % _patrolPoints.Length;

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

    /*void MoveToTarget()
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
            if (_state == DroneStates.Chase)
            {
                _rb.AddForce(direction * _chaseSpeed, ForceMode.Force);
            }
            else
            { 
                _rb.AddForce(direction * _patrolSpeed, ForceMode.Force);
            }
        }

        

        if (_state == DroneStates.Patrol &&
            (_currentTarget - transform.position).sqrMagnitude < _arrivalDistance * _arrivalDistance)
        {
            _currentTarget = GetNextTarget();
        }
    }*/

    Vector3 CalculateAvoidanceDirection(Vector3 desiredDirection)
    {
        if (_obstacleAvoidanceMask == 0)
            return desiredDirection;

        Vector3 origin = _eyePoint.position;
        Vector3 avoidance = Vector3.zero;
        bool centerBlocked = false;

        if (Physics.Raycast(origin, desiredDirection, out RaycastHit centerHit, _avoidanceRayLength, _obstacleAvoidanceMask))
        {
            centerBlocked = true;
        }

        Vector3 right = Vector3.Cross(Vector3.up, desiredDirection).normalized;
        Vector3 left = -right;

        float rightClearance = GetClearance(origin, desiredDirection, right);
        float leftClearance = GetClearance(origin, desiredDirection, left);

        if (centerBlocked)
        {
            Vector3 bestSide = (rightClearance > leftClearance) ? right : left;
            float clearance = Mathf.Max(rightClearance, leftClearance);

            float urgency = 1f - Mathf.Clamp01(clearance / _avoidanceRayLength);
            avoidance += bestSide * _avoidanceStrength * urgency;
        }

        Vector3 result = (desiredDirection + avoidance).normalized;
        return result;
    }

    float GetClearance(Vector3 origin, Vector3 forward, Vector3 side)
    {
        Vector3 dir = (forward + side * _avoidanceSideOffset).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, _avoidanceRayLength, _obstacleAvoidanceMask))
            return hit.distance;

        return _avoidanceRayLength;
    }

    void MoveToTarget()
    {
        if (_state == DroneStates.Idle)
        {
            Vector3 idleVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, _acceleration * Time.fixedDeltaTime);
            idleVelocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = idleVelocity;
            return;
        }

        Vector3 desiredDirection = (_currentTarget - _eyePoint.position).normalized;

        Vector3 steerDirection = CalculateAvoidanceDirection(desiredDirection);

        Vector3 flatDirection = _currentTarget - transform.position;
        flatDirection.y = 0;
        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        float targetSpeed = (_state == DroneStates.Chase) ? _chaseSpeed : _patrolSpeed;

        _rb.linearVelocity = steerDirection * targetSpeed;

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

    /*private void OnDrawGizmos()
    {
        //fov
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _fovDistance);

        *//*// радиус взрыва
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);*//*
    }*/

    private void OnDrawGizmos()
    {
        if (_eyePoint == null)
            return;

        Vector3 eyePos = _eyePoint.position;

        // FOV
        if (_droneLight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(eyePos, _droneLight.transform.forward * _fovDistance);

            Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
            Gizmos.DrawFrustum(eyePos, _fovAngle, _fovDistance, 0.1f, 1f);
        }

        if (_currentTarget == Vector3.zero || _obstacleAvoidanceMask == 0)
            return;

        Vector3 desiredDir = (_currentTarget - eyePos).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, desiredDir).normalized;
        Vector3 left = -right;

        Vector3 rightDir = (desiredDir + right * _avoidanceSideOffset).normalized;
        Vector3 leftDir = (desiredDir + left * _avoidanceSideOffset).normalized;

        bool centerHit = Physics.Raycast(eyePos, desiredDir, out RaycastHit centerInfo, _avoidanceRayLength, _obstacleAvoidanceMask);
        bool rightHit = Physics.Raycast(eyePos, rightDir, out RaycastHit rightInfo, _avoidanceRayLength, _obstacleAvoidanceMask);
        bool leftHit = Physics.Raycast(eyePos, leftDir, out RaycastHit leftInfo, _avoidanceRayLength, _obstacleAvoidanceMask);

        Gizmos.color = centerHit ? Color.red : Color.green;
        Gizmos.DrawLine(eyePos, eyePos + desiredDir * _avoidanceRayLength);
        if (centerHit)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(centerInfo.point, 0.1f);
        }

        Gizmos.color = rightHit ? Color.red : Color.yellow;
        Gizmos.DrawLine(eyePos, eyePos + rightDir * _avoidanceRayLength);
        if (rightHit)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rightInfo.point, 0.1f);
        }

        Gizmos.color = leftHit ? Color.red : Color.yellow;
        Gizmos.DrawLine(eyePos, eyePos + leftDir * _avoidanceRayLength);

        if (leftHit)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(leftInfo.point, 0.1f);
        }

        Vector3 steer = CalculateAvoidanceDirection(desiredDir);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(eyePos, eyePos + steer * _avoidanceRayLength);
        Gizmos.DrawSphere(eyePos + steer * _avoidanceRayLength, 0.08f);
    }
}