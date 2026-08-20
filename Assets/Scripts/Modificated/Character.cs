using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable, ISaveable
{
    [SerializeField] bool _isAlive = true;
    public bool IsAlive => _isAlive;

    [SerializeField] CharacterAnimationController _animationController;
    [SerializeField] CharacterInputController _inputController;
    [SerializeField] GroundRaycast _groundRaycast;
    [SerializeField] SlopeRaycast _slopeRaycast;
    [SerializeField] InteractRaycast _interactRaycast;
    [SerializeField] PushingRaycast _pushingRaycast;
    [SerializeField] CanStandRaycast _canStandUpRaycast;
    [SerializeField] Rigidbody _rb;
    [SerializeField] MovementAdvance[] _movements;//0 - walk, 1 - sprint, 2 - crouch, 3 - swim, 4 - slope, 5 - push
    [SerializeField] CharacterRotator _characterRotator;
    [SerializeField] Ragdoll _ragdoll;
    [SerializeField] Collider _col;
    [SerializeField] CharacterColliderResizer _characterColliderResizer;
    [SerializeField] FallDamage _fallDamage;
    [SerializeField] Transform _headPoint;
    [SerializeField] CharacterView _view;

    [SerializeField] PhysicsMaterial _slideMaterial;
    protected IExternalVelocity _externalVelocityProvider;
    protected Vector3 _externalVelocity;

    MovementAdvance _currentMovement;
    bool _isCrouching = false;
    bool _isSliding = false;
    bool _isSprinting = false;
    bool _isSwimming = false;
    bool _isPushing = false;
    bool _isGround = false;
    bool _isPressingNow = false;
    bool _isGrab = false;
    bool _isPushingNow = false;
    bool _inWaterZone = false;
    bool _isOnAir = false;
    bool _isFalling = false;

    bool _wasSwimming = false;
    bool _wasSliding = false;

    PushableBox _currentBox;
    public PushableBox CurrentBox => _currentBox;
    WaterZone _currentWaterZone;


    void Awake()
    {
        _characterColliderResizer.InitDefault();

        GameManager.Instance.Player = this;

        EventManager.Subscribe(EventType.OnFalled, HandleFallDeath);
        EventManager.Subscribe(EventType.OnFinishOxygen, InstantKill);
        SaveManager.Instance.Register(this);
    }

    void OnEnable()
    {
    }

    void Start()
    {
        ChangeMovement(_movements[0]);
    }

    void Update()
    {
        if (!_isAlive) return;

        _fallDamage.Tick(_isGround, _isSwimming, _isSliding, _rb.linearVelocity.y);

        _isGround = _groundRaycast.IsRaycasting(-Vector3.up);
        _isSliding = _slopeRaycast.IsRaycasting(-Vector3.up);
        _isGrab = _pushingRaycast.IsRaycasting(_characterRotator.Mesh.forward);
        _isOnAir = !_isGround && !_isSwimming;
        _animationController.SetBool(AnimParams.Air, _isOnAir);
        _animationController.SetBool(AnimParams.IsFalling, _isFalling);

        _isFalling = _rb.linearVelocity.y < -0.5f && !_isGround && !_isSwimming;

        _rb.maxLinearVelocity = _slopeRaycast.IsRaycasting(-Vector3.up) ? 15f : float.MaxValue;

        if (_currentWaterZone != null)
        {
            if (!_isSwimming && _inWaterZone && _headPoint.position.y < _currentWaterZone.BoundY)
            {
                EnterWater();
            }

            if (_isSwimming && _headPoint.position.y >= _currentWaterZone.BoundY + 0.1f && _isGround)
            {
                ExitWater();
            }
        }

        if (_inputController.IsInteracting)
        {
            if (_interactRaycast.IsRaycasting(_characterRotator.Mesh.forward))
            {
                Pressing();
            }

            _inputController.ResetInteract();
        }

        if (_inputController.IsJumping && _isGround && !_isCrouching && !_isSwimming)
        {
            _animationController.SetTrigger(AnimParams.Jump);

            if (_currentMovement.CurrentSpeed > 0.1f)
            {
                _currentMovement.Jump();
            }

            _inputController.ResetJump();
        }

        if (_inputController.IsCrouching)
        {
            _isCrouching = true;
        }
        else
        {
            if (_canStandUpRaycast.CanStandUp())
                _isCrouching = false;
        }

        _isSprinting = _inputController.IsSprinting;


        _animationController.SetFloat(AnimParams.Speed, _currentMovement.CurrentSpeed);

        if (_inputController.Direction.sqrMagnitude > 0.1f * 0.1f)
            _animationController.SetBool(AnimParams.Move, true);
        else
            _animationController.SetBool(AnimParams.Move, false);

        TryStartPush();

        if (_isPushingNow && !_inputController.IsPushing)
        {
            StopPush();
        }
    }

    void FixedUpdate()
    {
        if (!_isAlive) return;

        _externalVelocity = _externalVelocityProvider != null ? _externalVelocityProvider.ExternalVelocity : Vector3.zero;

        UpdateInputMap();

        if (_isSwimming)
        {
            ChangeMovement(_movements[3]);
        }
        else if (_isPushingNow)
        {
            ChangeMovement(_movements[5]);
        }
        else if (_isSliding)
        {
            ChangeMovement(_movements[4]);
        }
        else if (_isCrouching)
        {
            ChangeMovement(_movements[2]);
        }
        else if (_isSprinting)
        {
            ChangeMovement(_movements[1]);
        }
        else
        {
            ChangeMovement(_movements[0]);
        }

        TryCrouching();
        SlideCharacter();
        UpdateCollider();

        Vector3 dir = GetCurrentDirection();

        _currentMovement.Advance(dir, _externalVelocity);

        if (!_isPushingNow)
        {
            if (_isSwimming)
            {
                _characterRotator.RotateSwimming(dir);
            }
            else
            {
                _characterRotator.Rotate(_currentMovement.SmoothedDirection, _rb.linearVelocity);
            }
        }
    }

    private void OnDisable()
    {
    }

    void OnDestroy()
    {
        EventManager.Unsubscribe(EventType.OnFalled, HandleFallDeath);
        EventManager.Unsubscribe(EventType.OnFinishOxygen, InstantKill);
        SaveManager.Instance.Unregister(this);
    }

    void UpdateInputMap()
    {
        if (_isSwimming == _wasSwimming && _isSliding == _wasSliding) return;

        if (_isSwimming)
            _inputController.EnableMovementMap(); //change other direction Vector3
        else if (_isSliding)
            _inputController.EnableMovementMap(); //cgange to float forward/backward
        else
            _inputController.EnableMovementMap();

        _wasSwimming = _isSwimming;
        _wasSliding = _isSliding;
    }

    Vector3 GetCurrentDirection()
    {
        if (_isSwimming)
        {
            Vector3 swimDir = _inputController.SwimDirection;

            if (_headPoint.position.y >= _currentWaterZone.BoundY && swimDir.y > 0f)
            {
                swimDir.y = 0f;
            }

            return swimDir;
        }

        if (_isSliding)
        {
            return new Vector3(0f, 0f, _inputController.SlideAxis);
        }

        return _inputController.Direction;
    }

    void UpdateCollider()
    {
        if (_isCrouching)
        {
            _characterColliderResizer.SetSize(1f, new Vector3(0, 0.5f, 0));
        }
        else
        {
            _characterColliderResizer.SetSize(2f, new Vector3(0, 1f, 0));
        }
    }

    void HandleFallDeath(params object[] arg)
    {
        InstantKill();
    }

    public void InstantKill(params object[] parameters)
    {
        if (!_isAlive) return;

        _isAlive = false;
        DisableCharacter();
        ActivateDeathEffects();
        EventManager.Trigger(EventType.OnDead, _isAlive);
    }

    void DisableCharacter()
    {
        _rb.isKinematic = true;
        _col.enabled = false;
    }

    void ActivateDeathEffects()
    {
        _ragdoll.ActivateRagdoll();
        _ragdoll.ActivateCollision();

        _view.PlayBloodVFX();
        EventManager.Trigger(EventType.OnDead);
        _view.PlayVoice();
    }

    void ChangeMovement(MovementAdvance newMovement)
    {
        if (_currentMovement == newMovement) return;

        float prevSpeed = _currentMovement != null
        ? _currentMovement.CurrentSpeed
        : 0f;

        _currentMovement = newMovement;
        _currentMovement.Initialize(_rb);
        _currentMovement.SetSpeed(prevSpeed);

        _characterRotator.Initialize(_slopeRaycast);
    }

    public void SetExternalVelocity(IExternalVelocity velocity)
    {
        _externalVelocityProvider = velocity;
    }

    void TryCrouching()
    {
        if (!_isGround) return;

        _animationController.SetBool(AnimParams.Crouch, _isCrouching);
    }

    public void ChangePhysicMaterial()
    {
        _col.material = _slideMaterial;
    }

    public void ResetPhysicsMaterial()
    {
        _col.material = null;
    }

    void SlideCharacter()
    {
        _animationController.SetBool(AnimParams.Slide, _isSliding);
        _view.PlayDustVFX(_isSliding);
    }

    public void SetWaterZone(WaterZone waterZone)
    {
        _currentWaterZone = waterZone;
        _inWaterZone = waterZone != null;
    }

    public void EnterWater()
    {
        _isSwimming = true;
        _rb.useGravity = false;
        _animationController.SetTrigger(AnimParams.StartSwimm);

        ChangeMovement(_movements[3]);
        
        _view.PlayBubbleVFX(true);
    }

    public void ExitWater()
    {
        _isSwimming = false;
        _rb.useGravity = true;
        _animationController.SetTrigger(AnimParams.StopSwimm);

        ChangeMovement(_movements[0]);

        _view.PlayBubbleVFX(false);
    }

    public void Pressing()
    {
        if (_isPressingNow) return;

        _isPressingNow = true;

        _animationController.SetTrigger(AnimParams.Press);
        _interactRaycast.InteractPress();

        StartCoroutine(ResetPress());
    }

    void TryStartPush()
    {
        if (_isPushingNow) return;

        if (_inputController.IsPushing && _isGrab)
        {
            _pushingRaycast.InteractPress();
        }
    }


    public void StartPush(PushableBox box)
    {
        if (_isPushingNow) return;

        Debug.Log("Start Pushing");

        _isPushingNow = true;
        _currentBox = box;

        Vector3 dir = (box.transform.position - transform.position).normalized;
        dir.y = 0f;

        _characterRotator.Mesh.forward = dir;

        _animationController.SetBool(AnimParams.Push, true);
    }

    public void StopPush()
    {
        if (!_isPushingNow) return;
        Debug.Log("Stop Pushing");
        _isPushingNow = false;
        _currentBox = null;

        _animationController.SetBool(AnimParams.Push, false);
    }

    IEnumerator ResetPress()
    {
        yield return new WaitForSeconds(2f);
        _isPressingNow = false;
    }

    public void DeactivateRBKinematic()
    {
        _rb.isKinematic = false;
    }

    public void ActivateRBKinematic()
    {
        _rb.isKinematic = true;
        _rb.linearVelocity = Vector3.zero;
    }

    public void PlayJump()
    {
        _currentMovement.Jump();
    }

    public void CaptureState(SaveGameData data)
    {
        data.x = transform.position.x;
        data.y = transform.position.y;
        data.z = transform.position.z;
        data.IsAlive = _isAlive;
        data.СurrentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public void RestoreState(SaveGameData data)
    {
        Vector3 pos = new Vector3(data.x, data.y, data.z);
        transform.position = pos;
        _isAlive = data.IsAlive;
    }
}