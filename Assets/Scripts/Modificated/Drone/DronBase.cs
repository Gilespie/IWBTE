using UnityEngine;
using UnityEngine.AI;
public enum PatrolLookMode
{
    Fixed,          
    FaceMovement
}

public class DronBase : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] protected float _rotationSpeed = 5f;
    [SerializeField] PatrolLookMode _patrolLookMode = PatrolLookMode.Fixed;
    [SerializeField] float _patrolRotationSpeed = 3f;
    [SerializeField] float _returnRotationSpeed = 3f;

    [Header("Movement")]
    [SerializeField] protected float _patrolSpeed = 5f;
    [SerializeField] protected float _chaseSpeed = 8f;
    [SerializeField] protected float _arrivalDistance = 1f;

    [Header("AI")]
    [SerializeField] protected Transform _target;
    [SerializeField] protected Transform _eyePoint;

    [Header("FOV")]
    [SerializeField] protected LayerMask _obstacleMask;

    [Header("AI - Light")]
    [SerializeField] protected Light _droneLight;
    [SerializeField] protected Color _patrolColor = Color.cyan;
    [SerializeField] protected Color _chaseColor = Color.red;
    [SerializeField] protected Color _searchColor = Color.yellow;
    [SerializeField] protected bool _startIdle = true;

    [Header("SFX")]
    [SerializeField] protected AudioClip _detectClip;
    [SerializeField] protected AudioSource _detectAudioSource;
    [SerializeField] float _minBeepInterval = 0.1f;
    [SerializeField] float _maxBeepInterval = 2f;
    [SerializeField] float _maxBeepDistance = 15f;

    [Header("Patrol")]
    [SerializeField] protected Transform[] _patrolPoints;
    [SerializeField] protected bool _randomPatrol = true;
    [SerializeField] Vector2 _waitTimeRange = new Vector2(2f, 4f);

    [Header("NavMesh")]
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] float _hoverHeight = 2f;
    [SerializeField] float _sampleRadius = 3f;
    Vector3 _lastDestination = Vector3.positiveInfinity;

    [Header("Targeting")]
    [SerializeField] LayerMask _targetMask;
    [SerializeField] float _scanInterval = 0.2f;
    readonly Collider[] _scanBuffer = new Collider[16];
    Quaternion _defaultRotation;
    float _scanTimer;
    AimTargetStruct _targetAim;
    protected DroneStates _state = DroneStates.Patrol;
    protected bool _seesTarget;          
    protected Vector3 _currentTarget;
    protected Vector3 _lastSeenPosition;
    protected IDamageable _currentTargetDamageable;
    float _fovDistance, _fovAngle;
    int _currentIndex;
    float _beepTimer;
    float _waitTimer;
    bool _waiting;

    protected virtual void Awake()
    {
        if (_agent == null) return;

        _agent.updateRotation = false;      
        _agent.baseOffset = _hoverHeight;   
    }

    protected virtual void Start()
    {
        _defaultRotation = transform.rotation;

        if (_target == null && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            SetTarget(GameManager.Instance.Player.transform);
        }
        else if (_target != null)
        {
            SetTarget(_target);
        }

        _fovAngle = _droneLight.spotAngle;
        _fovDistance = _droneLight.range;

        if (_startIdle)
        {
            _state = DroneStates.Idle;
        }
        else
        {
            _currentIndex = _randomPatrol ? Random.Range(0, _patrolPoints.Length) : 0;
            _currentTarget = _patrolPoints[_currentIndex].position;
            _state = DroneStates.Patrol;
        }

        SetLightColor(_patrolColor);
    }

    protected virtual void FixedUpdate()
    {
        _scanTimer -= Time.fixedDeltaTime;
        if (_scanTimer <= 0f)
        {
            _scanTimer = _scanInterval;
            AcquireTarget();
        }

        _seesTarget = CanSeeTarget(_target);

        UpdateState();
        MoveToTarget();
        Tick();
        HandleBeepSound(_target);
    }

    protected virtual void Tick() { }

    protected virtual void OnEnterChase() { }
    protected virtual void OnEnterInvestigate() { }
    protected virtual void OnEnterPatrol() { }
    protected virtual void OnEnterIdle() { }

    protected virtual bool ShouldHoldPosition() => false;

    void AcquireTarget()
    {
        
        if (_target != null && _seesTarget) return;

        int count = Physics.OverlapSphereNonAlloc(
            _eyePoint.position, _fovDistance, _scanBuffer, _targetMask, QueryTriggerInteraction.Ignore);

        Transform best = null;
        float bestSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            IDamageable dmg = _scanBuffer[i].GetComponentInParent<IDamageable>();
            if (dmg == null || !dmg.IsAlive) continue;

            Transform t = ((Component)dmg).transform;
            if (t.IsChildOf(transform)) continue;          
            if (!CanSeeTarget(t, dmg)) continue;

            float sqr = (t.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; best = t; }
        }

        if (best != null && best != _target)
            SetTarget(best);
    }

    void UpdateState()
    {
        switch (_state)
        {
            case DroneStates.Idle:
            case DroneStates.Patrol:
                if (_seesTarget) EnterChase();
                break;

            case DroneStates.Chase:
                if (_seesTarget)
                {
                    _lastSeenPosition = _target.position;
                    _currentTarget = _target.position;
                }
                else EnterInvestigate();
                break;

            case DroneStates.Investigate:
                if (_seesTarget) EnterChase();
                else if (IsNear(_lastSeenPosition))
                {
                    if (_startIdle) EnterIdle(); else EnterPatrol();
                }
                else _currentTarget = _lastSeenPosition;
                break;
        }
    }

    bool IsNear(Vector3 point)
    {
        Vector3 d = transform.position - point;
        d.y = 0f;
        return d.sqrMagnitude < _arrivalDistance * _arrivalDistance;
    }

    void EnterChase()
    {
        _state = DroneStates.Chase;
        SetLightColor(_chaseColor);
        OnEnterChase();
    }

    void EnterInvestigate()
    {
        _state = DroneStates.Investigate;
        _currentTarget = _lastSeenPosition;
        SetLightColor(_searchColor);
        OnEnterInvestigate();
    }

    void EnterPatrol()
    {
        _state = DroneStates.Patrol;
        _lastSeenPosition = Vector3.zero;
        _currentTarget = GetNextTarget();
        SetLightColor(_patrolColor);
        OnEnterPatrol();
    }

    void EnterIdle()
    {
        _state = DroneStates.Idle;
        if (_agent.enabled && _agent.isOnNavMesh) _agent.ResetPath();
        _lastDestination = Vector3.positiveInfinity;
        SetLightColor(_patrolColor);
        OnEnterIdle();
    }
    protected bool CanSeeTarget(Transform target) => CanSeeTarget(target, _currentTargetDamageable);

    protected bool CanSeeTarget(Transform target, IDamageable owner)
    {
        if (target == null || owner == null || !owner.IsAlive) return false;

        Vector3 point = (target == _target ? _targetAim : new AimTargetStruct(target)).Position;
        Vector3 eyePos = _eyePoint.position;

        if ((point - eyePos).sqrMagnitude > _fovDistance * _fovDistance)
            return false;

        Vector3 dir = (point - eyePos).normalized;
        if (Vector3.Angle(_droneLight.transform.forward, dir) > _fovAngle * 0.5f)
            return false;

        if (Physics.Linecast(eyePos, point, out RaycastHit hit, _obstacleMask, QueryTriggerInteraction.Ignore))
            return hit.collider.GetComponentInParent<IDamageable>() == owner;

        return true;
    }

    Vector3 GetNextTarget()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return transform.position;

        _currentIndex = _randomPatrol
            ? Random.Range(0, _patrolPoints.Length)
            : (_currentIndex + 1) % _patrolPoints.Length;

        return _patrolPoints[_currentIndex].position;
    }

    void MoveToTarget()
    {
        if (!_agent.enabled || !_agent.isOnNavMesh) return;

        bool hold = _state == DroneStates.Idle || ShouldHoldPosition();

        if (hold)
        {
            _agent.isStopped = true;

            if (ShouldHoldPosition()) RotateTowards(_currentTarget, _rotationSpeed);
            else RotateToDefault(_returnRotationSpeed);  
            return;
        }

        _agent.isStopped = false;
        _agent.speed = _state == DroneStates.Chase ? _chaseSpeed : _patrolSpeed;

        SetDestination(_currentTarget);
        UpdateRotation();

        if (_state == DroneStates.Patrol) PatrolWait();
    }
    void UpdateRotation()
    {
        switch (_state)
        {
            case DroneStates.Chase:
                RotateTowards(_currentTarget, _rotationSpeed);
                break;

            case DroneStates.Investigate:
                RotateTowards(_agent.steeringTarget, _rotationSpeed);
                break;

            case DroneStates.Patrol:
                if (_patrolLookMode == PatrolLookMode.FaceMovement)
                    RotateTowards(_agent.steeringTarget, _patrolRotationSpeed);
                else
                    RotateToDefault(_patrolRotationSpeed);
                break;
        }
    }

    void SetDestination(Vector3 point)
    {
        if ((point - _lastDestination).sqrMagnitude < 0.25f) return;

        if (NavMesh.SamplePosition(point, out NavMeshHit hit, _sampleRadius, NavMesh.AllAreas))
        {
            _lastDestination = point;
            _agent.SetDestination(hit.position);
        }
    }

    void RotateTowards(Vector3 point, float speed)
    {
        Vector3 flat = point - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(flat),
            speed * Time.fixedDeltaTime);
    }

    void RotateToDefault(float speed)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, _defaultRotation, speed * Time.fixedDeltaTime);
    }

    void PatrolWait()
    {
        if (_waiting)
        {
            _agent.isStopped = true;
            _waitTimer -= Time.fixedDeltaTime;

            if (_waitTimer <= 0f)
            {
                _waiting = false;
                _currentTarget = GetNextTarget();
            }
        }
        else if (IsNear(_currentTarget))
        {
            _waiting = true;
            _waitTimer = Random.Range(_waitTimeRange.x, _waitTimeRange.y);
        }
    }

    protected void HandleBeepSound(Transform to)
    {
        if (_state != DroneStates.Chase || to == null) return;

        float t = Mathf.Clamp01(Vector3.Distance(transform.position, to.position) / _maxBeepDistance);
        _beepTimer -= Time.fixedDeltaTime;

        if (_beepTimer <= 0f)
        {
            _detectAudioSource.PlayOneShot(_detectClip);
            _beepTimer = Mathf.Lerp(_minBeepInterval, _maxBeepInterval, t);
        }
    }

    protected void SetLightColor(Color c)
    {
        if (_droneLight != null) _droneLight.color = c;
    }

    public virtual void SetTarget(Transform target)
    {
        if (target == null)
        {
            _target = null;
            _targetAim = default;
            _currentTargetDamageable = null;
            return;
        }

        IDamageable damageable = target.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            _target = null;
            _currentTargetDamageable = null;
            return;
        }

        _target = target;
        _currentTargetDamageable = damageable;
        _targetAim = new AimTargetStruct(_target);
    }
}
