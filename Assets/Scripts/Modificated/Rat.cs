using UnityEngine;

public class Rat : MonoBehaviour, IDamageable
{
    enum State { Idle, Scurry, Flee }

    [Header("References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Transform _mesh;
    [SerializeField] Animator _animator;

    [Header("Movement")]
    [SerializeField] bool _isCutsceneRat = false;
    [SerializeField] float _scurrySpeed = 3f;
    [SerializeField] float _fleeSpeed = 6f;
    [SerializeField] float _acceleration = 20f;
    [SerializeField] float _turnSpeed = 12f;
    [SerializeField] bool _planarSideScroll = true;

    [Header("Stop & Go")]
    [SerializeField] Vector2 _scurryTime = new(0.4f, 1.2f);
    [SerializeField] Vector2 _idleTime = new(0.3f, 1.5f);
    [SerializeField, Range(0f, 1f)] float _turnAroundChance = 0.3f;

    [Header("Detection")]
    [SerializeField] float _fleeDistance = 4f;
    [SerializeField] LayerMask _obstacleMask;
    [SerializeField] float _wallCheckDistance = 0.6f;
    [SerializeField] float _edgeCheckForward = 0.5f;
    [SerializeField] float _edgeCheckDepth = 1f;

    State _state = State.Idle;
    Vector3 _dir = Vector3.right;
    float _stateTimer;
    float _currentSpeed;
    bool _isAlive = true;
    public bool IsAlive => _isAlive;

    void Start()
    {
        if (_isCutsceneRat)
        {
            _rb.isKinematic = true;          
            _dir = Vector3.right;
            _currentSpeed = _scurrySpeed;

            if (_mesh != null)
                _mesh.rotation = Quaternion.LookRotation(_dir, Vector3.up);

            if (_animator != null)
                _animator.SetFloat(AnimParams.Speed, _currentSpeed);

            return;
        }

        if (_planarSideScroll) _dir = Random.value > 0.5f ? Vector3.right : Vector3.left;
        else _dir = RandomDirection();

        SetState(State.Idle);
    }

    void Update()
    {
        if (!_isAlive || _isCutsceneRat) return;

        _stateTimer -= Time.deltaTime;

        if(_isCutsceneRat)
        {
            _rb.MovePosition(_rb.position + Vector3.right * _currentSpeed * Time.deltaTime);
            return;
        }

        Character player = GameManager.Instance.Player;
        bool playerNear = player != null && player.IsAlive &&
            (player.transform.position - transform.position).sqrMagnitude <= _fleeDistance * _fleeDistance;

        if (playerNear)
        {
            _dir = GetAwayDirection(player.transform.position);
            if (_state != State.Flee) SetState(State.Flee);
        }
        else if (_state == State.Flee)
        {
            SetState(State.Idle);
        }

        switch (_state)
        {
            case State.Idle:
                if (_stateTimer <= 0f) SetState(State.Scurry);
                break;

            case State.Scurry:
                if (_stateTimer <= 0f) SetState(State.Idle);
                break;
        }

        if (_animator != null)
            _animator.SetFloat(AnimParams.Speed, _currentSpeed);
    }

    void FixedUpdate()
    {
        if (!_isAlive) return;

        if (_isCutsceneRat)
        {
            _rb.MovePosition(_rb.position + _dir * _currentSpeed * Time.fixedDeltaTime);
            return;
        }

        float targetSpeed = _state switch
        {
            State.Scurry => _scurrySpeed,
            State.Flee => _fleeSpeed,
            _ => 0f
        };

        if (targetSpeed > 0f && IsBlocked())
        {
            TurnAround();
            if (_state == State.Flee) targetSpeed = 0f;
        }

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.fixedDeltaTime);

        Vector3 vel = _dir * _currentSpeed;
        vel.y = _rb.linearVelocity.y; 
        _rb.linearVelocity = vel;

        RotateMesh();
    }

    void SetState(State state)
    {
        _state = state;

        switch (state)
        {
            case State.Idle:
                _stateTimer = Random.Range(_idleTime.x, _idleTime.y);
                break;

            case State.Scurry:
                _stateTimer = Random.Range(_scurryTime.x, _scurryTime.y);

                if (Random.value < _turnAroundChance) TurnAround();
                else if (!_planarSideScroll) _dir = RandomDirection();
                break;
        }
    }

    bool IsBlocked()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, _dir, _wallCheckDistance, _obstacleMask, QueryTriggerInteraction.Ignore))
            return true;

        Vector3 edgeOrigin = origin + _dir * _edgeCheckForward;
        bool hasGround = Physics.Raycast(edgeOrigin, Vector3.down, _edgeCheckDepth, _obstacleMask, QueryTriggerInteraction.Ignore);

        return !hasGround;
    }

    void TurnAround()
    {
        _dir = -_dir;
    }

    Vector3 GetAwayDirection(Vector3 from)
    {
        Vector3 away = transform.position - from;
        away.y = 0f;

        if (_planarSideScroll)
            return new Vector3(Mathf.Sign(away.x == 0 ? 1f : away.x), 0f, 0f);

        return away.sqrMagnitude > 0.001f ? away.normalized : _dir;
    }

    Vector3 RandomDirection()
    {
        Vector2 c = Random.insideUnitCircle.normalized;
        return new Vector3(c.x, 0f, c.y);
    }

    void RotateMesh()
    {
        if (_mesh == null || _dir.sqrMagnitude < 0.001f) return;

        Quaternion target = Quaternion.LookRotation(_dir, Vector3.up);
        _mesh.rotation = Quaternion.Slerp(_mesh.rotation, target, _turnSpeed * Time.fixedDeltaTime);
    }

    public void InstantKill(params object[] parameters)
    {
        if (!_isAlive) return;

        _isAlive = false;
        if (!_rb.isKinematic) _rb.linearVelocity = Vector3.zero;
        if (_animator != null) _animator.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _fleeDistance);

        Gizmos.color = Color.red;
        Vector3 o = transform.position + Vector3.up * 0.2f;
        Gizmos.DrawLine(o, o + _dir * _wallCheckDistance);
        Gizmos.DrawLine(o + _dir * _edgeCheckForward, o + _dir * _edgeCheckForward + Vector3.down * _edgeCheckDepth);
    }
}