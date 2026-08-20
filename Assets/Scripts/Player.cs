using System.Collections;
using UnityEngine;

public class Player : Destructable
{
    [Header("Reference")]
    [SerializeField] private Transform _mesh;

    [Header("Inputs")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _crouchKey = KeyCode.C;
    [SerializeField] private KeyCode _pushingKey = KeyCode.E;
    [SerializeField] private KeyCode _pressingKey = KeyCode.F;

    [Header("Animator")]
    [SerializeField] private string _moveBoolName = "isMoving";
    [SerializeField] private string _airBoolName = "isOnAir";
    [SerializeField] private string _jumpTriggerName = "onJump";
    [SerializeField] private string _crouchBoolName = "isCrouch";
    [SerializeField] private string _pushingBoolName = "isPushing";
    [SerializeField] private string _pressTriggerName = "onPressed";
    [SerializeField] private string _xAxisName = "xAxis";
    [SerializeField] private string _zAxisName = "zAxis";
    [SerializeField] private string _moveStateName = "moveState";
    [SerializeField] private string _slopeBoolName = "isSliding";

    [Header("Physics")]
    [SerializeField] private Raycasting _raycast;
    [SerializeField] private Ragdoll _ragdoll;
    [SerializeField] private RotationTransform _rotationTransform;
    [SerializeField] private PhysicsMaterial _physicsMaterial;

    [Header("Parameters")]
    [SerializeField] private float _speedMultiplier = 1f;
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _slopeSpeed = 8f;
    [SerializeField] private float _crouchSpeed = 5f;
    [SerializeField] private float _pushingSpeed = 1f;
    [SerializeField] private float _jumpForce = 25f;
    [SerializeField] private CameraFollower _follower;
    [SerializeField] private bool _isGrounded = false;
    private bool _isCrouch = false;
    private bool _isPushing = false;
    private bool _isOnce = false;
    private bool _canMove = false;
    public bool CanMove => _canMove;

    private bool _isInteractable = false;
    private bool _isSlope = false;
    private bool _isCeiling = false;

    [Header("Falling")]
    [SerializeField] private float _fallDamageMultiplier = 100f;
    [SerializeField] private float _fallDamageThreshold = -10f;

    [Header("Swimming")]
    [SerializeField] private float _swimSpeed = 2f;
    [SerializeField] private float _swimUpForce = 5f;
    [SerializeField] private string _startSwimTriggerName = "onStartSwimming";
    [SerializeField] private string _stopSwimTriggerName = "onStopSwimming";
    [SerializeField] private ParticleSystem[] _bubleParticles;
    [SerializeField] private Transform _headPoint;
    [SerializeField] private float _waterCheckDistance = 0.2f;
    [SerializeField] private LayerMask _waterLayer;
    [SerializeField] private AirManager _airManager;

    private Coroutine _slowFallCoroutine;
    private float _slowTime = 0f;
    private bool _isSwimming = false;

    [Header("SFX&VFX")]
    [SerializeField] private VFXSpawner _spawner;
    [SerializeField] private AudioClip _footstep;
    [SerializeField] private AudioClip[] _voices;
    private AudioSource _audioSource;

    //public DeathScreenManager deathScreenManager;
    [SerializeField] private float _secondsUntilRestart = 3f;

    private bool _wasGround = false;
    private bool _inWaterZone = false;
    private float _maxFallSpeed = 0f;
    private float _fallDamage = 0f;
    private bool _isEndGame = false;
    private bool _wasInAir = false;
    private bool _wasSwimming = false;

    private float _currentSpeed = 0f;
    private Vector3 _direction;

    private Quaternion _defaultRotation;
    private Rigidbody _rb;
    private CapsuleCollider _col;
    private Animator _animator;
    private WaterZone _currentWater;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();

        //GameManager.Instance.Player = this;
    }

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnFinishOxygen, DeactivatePlayer);
    }

    void Start()
    {
        _defaultRotation = _mesh.rotation;

        foreach (var particle in _bubleParticles)
        {
            particle.Stop();
        }

        if (GameManager.Instance.ActualCheckpoint == Vector3.zero || GameManager.Instance != null)
            GameManager.Instance.ActualCheckpoint = transform.position;
        else
            transform.position = GameManager.Instance.ActualCheckpoint;
    }


    void Update()
    {
        CalculateFallDamage();

        if (!_isSwimming && _inWaterZone && _headPoint.position.y < _currentWater.BoundY)
        {
            EnterWater();
        }

        if (_isSwimming && _headPoint.position.y >= _currentWater.BoundY && _raycast.IsGrounded())
        {
            ExitWater();
        }

        if (_isSwimming)
        {
            _direction.x = Input.GetAxis("Horizontal");
            _direction.y = Input.GetAxis("Jump") > 0 ? 1 : (Input.GetKey(KeyCode.LeftControl) ? -1 : 0);
            _direction.z = Input.GetAxis("Vertical");

            _animator.SetBool(_moveBoolName, _direction.sqrMagnitude > 0.01f);
            _animator.SetFloat(_xAxisName, _direction.x);
            _animator.SetFloat(_zAxisName, _direction.z);

            return;
        }

        HandleGroundChecks();

        HandleInput();

        HandleStateChanges();

        HandleSpeed();
    }

    void FixedUpdate()
    { 
        if (!_isAlive) return;

        if (_isSwimming)
        {
            SwimPlayer(_direction);
            return;
        }

        if (_direction.sqrMagnitude != 0.0f && _isAlive)
        {
            MovePlayer(_direction);
        }
        else if (_direction.sqrMagnitude != 0.0f && _isAlive && _isSlope)
        {
            SlopeMovement(_direction);
        }
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnFinishOxygen, DeactivatePlayer);
    }

    public void ResetPlayer()
    {
        transform.position = GameManager.Instance.ActualCheckpoint;
    }

    public void SetWaterZone(WaterZone water)
    {
        _currentWater = water;
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(_jumpKey) && _isGrounded && !_isCrouch && !_isSlope)
        {
            _animator.SetTrigger(_jumpTriggerName);
            JumpPlayer();
        }

        if (Input.GetKeyDown(_crouchKey) && _isGrounded && !_isCeiling)
        {
            _isCrouch = !_isCrouch;
        }

        if (Input.GetKeyDown(_pushingKey) && _isGrounded && _isInteractable)
        {
            _animator.SetTrigger(_pressTriggerName);
            Pressing();
        }

        if (Input.GetKey(_pushingKey) && _isGrounded && _isPushing && !_isCrouch)
        {
            if (!_canMove) Pushing();
        }
        else
        {
            if (_canMove) StopPushing();
        }
    }

    private void HandleGroundChecks()
    {
        _isGrounded = _raycast.IsGrounded();
        _isInteractable = _raycast.IsInteract();
        _isPushing = _raycast.IsPushing();
        _isSlope = _raycast.IsSlope();
        _isCeiling = _raycast.IsCeiling();

        _animator.SetBool(_airBoolName, !_isGrounded);
        _animator.SetBool(_moveBoolName, _direction.sqrMagnitude != 0f);
        _animator.SetBool(_slopeBoolName, _isSlope);
        _animator.SetFloat(_xAxisName, _direction.x);
        _animator.SetFloat(_zAxisName, _direction.z);

        _col.sharedMaterial = _isSlope ? _physicsMaterial : null;
        _rb.maxLinearVelocity = _isSlope ? 15f : float.MaxValue;

        if (_isSlope)
            SlopeRotateMesh();

        if (_isSlope)
        {
            _direction.x = 0f;
            _direction.z = Input.GetAxis("Vertical");
        }
        else
        {
            _direction.x = Input.GetAxis("Horizontal");
            _direction.z = Input.GetAxis("Vertical");
        }
    }

    private void HandleStateChanges()
    {
        if (_isCrouch)
            Crouch();
        else
            Uncrouch();
    }

    private void HandleSpeed()
    {
        if (_canMove)
            ChangeSpeed(_pushingSpeed);
        else if (_isCrouch)
            ChangeSpeed(_crouchSpeed);
        else if (_isSlope)
            ChangeSpeed(_slopeSpeed);
        else
            ChangeSpeed(_moveSpeed);
    }

    public void SetInWaterZone(bool state)
    {
        _inWaterZone = state;
    }

    public void SlopeRotateMesh()
    {
        Vector3 slopeNormal = _raycast.Normal;
        Vector3 forward = _rb.linearVelocity;

        Quaternion slopeTilt = Quaternion.FromToRotation(Vector3.up, slopeNormal);
        Vector3 adjustedForward = Vector3.ProjectOnPlane(forward, slopeNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(adjustedForward, slopeNormal);

        _mesh.rotation = targetRotation;
    }

    public void SlopeMovement(Vector3 dir)
    {
        Vector3 moveDir = (transform.forward * dir.z).normalized;
        _rb.MovePosition(transform.position + moveDir * _currentSpeed * Time.fixedDeltaTime);
    }

    public float ChangeSpeed(float speed)
    {
        _currentSpeed = speed * _speedMultiplier;
        return _currentSpeed;
    }

    public void SetSpeedMultiplier(float value)
    {
        _speedMultiplier = value;
    }

    public void EnterWater()
    {
        _isSwimming = true;
        _rb.useGravity = false;

        _animator.SetTrigger(_startSwimTriggerName);

        foreach (var particle in _bubleParticles)
        {
            particle.Play();
        }
    }

    public void ExitWater()
    {
        _isSwimming = false;
        _rb.useGravity = true;
        _animator.SetTrigger(_stopSwimTriggerName);

        foreach (var particle in _bubleParticles)
        {
            particle.Stop();
        }
    }

    private void CalculateFallDamage()
    {
        if (!_isGrounded && _isSwimming)
        {
            _maxFallSpeed = 0f;
            return;
        }

        if (!_isGrounded && !_isSwimming)
        {
            if (_rb.linearVelocity.y < _maxFallSpeed)
            {
                _maxFallSpeed = _rb.linearVelocity.y;
            }
        }

        if (_isGrounded && !_wasGround)
        {
            if (_maxFallSpeed < _fallDamageThreshold)
            {
                _fallDamage = Mathf.Abs(_maxFallSpeed + _fallDamageThreshold) * _fallDamageMultiplier;
                InstantKill();
            }

            _maxFallSpeed = 0f;
        }

        _wasGround = _isGrounded;
    }

    private void MovePlayer(Vector3 dir)
    {
        Vector3 moveDir = (transform.right * dir.x + transform.forward * dir.z).normalized;
        _rb.MovePosition(_rb.position + moveDir * _currentSpeed * Time.fixedDeltaTime);
    }

    private void SwimPlayer(Vector3 dir)
    {
        Vector3 swimDir = Vector3.zero;

        if (_headPoint.position.y >= _currentWater.BoundY && dir.y > 0f)
        {
            dir.y = 0f;
        }

        swimDir = (transform.right * dir.x + transform.up * dir.y + transform.forward * dir.z).normalized;
        _rb.MovePosition(_rb.position + swimDir * _swimSpeed * Time.fixedDeltaTime);
    }

    public float GetSpeed()
    {
        return _currentSpeed;
    }

    private void JumpPlayer()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Crouch()
    {
        _col.height = 1;
        _col.center = new Vector3(_col.center.x, 0.5f, _col.center.z);
        _animator.SetBool(_crouchBoolName, true);
    }

    private void Uncrouch()
    {
        _animator.SetBool(_crouchBoolName, false);
        _col.height = 2;
        _col.center = new Vector3(_col.center.x, 1f, _col.center.z);
    }

    public void Pushing()
    {
        if (_canMove) return;

        _canMove = true;
        _raycast.Interact(); 
        _raycast.Pushing();
        _rotationTransform.enabled = false;
        _animator.SetBool(_pushingBoolName, _canMove);
    }

    public void StopPushing()
    {
        _canMove = false;
        _raycast.Interact();
        _raycast.Pushing();
        _rotationTransform.enabled = true;
        _animator.SetBool(_pushingBoolName, _canMove);
    }    

    public void Pressing()
    {
        _raycast.Interact();
    }

    public void ActivateControl()
    {
        this.enabled = true;
        _rotationTransform.enabled = true;
    }

    public void DeactivateControl()
    {
        this.enabled = false;
        _rotationTransform.enabled = false;
    }

    void DeactivatePlayer(params object[] parameters)
    {
        if (_isOnce) return;

        _isAlive = false;
        _rb.isKinematic = true;
        _col.enabled = false;
        _rotationTransform.enabled = false;
        _ragdoll.ActivateRagdoll();
        _ragdoll.ActivateCollision();
        PlayVoice();

        if (_isEndGame)
        {
            _spawner.SpawnParticle(transform);
            EventManager.Trigger(EventType.OnEndGame);
        }
        else if(!_isOnce)
        {
            _spawner.SpawnParticle(transform);
            StartCoroutine(GameOverPanel());
            _isOnce = true;
        }
        this.enabled = false;
    }

    private IEnumerator GameOverPanel()
    {
        yield return new WaitForSeconds(_secondsUntilRestart);
        Die();

        yield return null;
    }

    public void Die()
    {
        //deathScreenManager.ActivateFadeIn();
    }

    public void PlayFootStep()
    {
        _audioSource.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        _audioSource.PlayOneShot(_footstep);
    }

    public void PlayVoice()
    {
        int index = UnityEngine.Random.Range(0, _voices.Length);
        _audioSource.PlayOneShot(_voices[index]);
    }

    public void SetEndGame()
    {
        _isEndGame = true;
    }

    public override void InstantKill(params object[] parameters)
    {
        base.InstantKill(parameters);

        EventManager.Trigger(EventType.OnDead, _isAlive);
        DeactivatePlayer();
    }

    /*    private void OnDrawGizmos()
        {
            if (_headPoint == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                _headPoint.position,
                _headPoint.position + Vector3.up * _waterCheckDistance
            );

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_headPoint.position, _headPoint.position + Vector3.up * _waterCheckDistance);
        }*/
}