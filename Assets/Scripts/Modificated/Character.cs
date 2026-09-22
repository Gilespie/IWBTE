using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable, ISaveable
{
    [SerializeField] bool _isAlive = true;
    public bool IsAlive => _isAlive;

    [SerializeField] GrabRigController _handsGrabRig;
    [SerializeField] GrabOneHandController _oneHandGrabRig;

    [SerializeField] CharacterAnimationController _animationController;
    [SerializeField] CharacterInputController _inputController;
    [SerializeField] GroundRaycast _groundRaycast;
    [SerializeField] ForwardRaycast _interactRaycast;
    [SerializeField] CanStandRaycast _canStandUpRaycast;
    [SerializeField] Rigidbody _rb;
    [SerializeField] GroundMovement _movement;
    //[SerializeField] MovementAdvance[] _movements;//0 - walk, 1 - sprint, 2 - crouch, 3 - swim, 4 - slope, 5 - push
    [SerializeField] CharacterRotator _characterRotator;
    [SerializeField] Ragdoll _ragdoll;
    [SerializeField] Collider _col;
    [SerializeField] CharacterColliderResizer _characterColliderResizer;
    [SerializeField] FallDamage _fallDamage;
    [SerializeField] Transform _headPoint;
    [SerializeField] CharacterView _view;

    [SerializeField] PhysicsMaterial _slideMaterial;
   // protected IExternalVelocity _externalVelocityProvider;
   // protected Vector3 _externalVelocity;

    //MovementAdvance _currentMovement;
    bool _isCrouching = false;
    bool _isSliding = false;
    bool _isSprinting = false;
    bool _isSwimming = false;
    //bool _isPushing = false;
    bool _isGround = false;
    bool _isPressingNow = false;
    bool _isGrab = false;
    bool _isPushingNow = false;
    bool _inWaterZone = false;
    bool _isOnAir = false;
    bool _isFalling = false;
    bool _isWalking = false;
    //bool _wasSwimming = false;
    //bool _wasSliding = false;

    PushableBox _currentBox;
    public PushableBox CurrentBox => _currentBox;
    WaterZone _currentWaterZone;
    //private IPushable _currentPushable;

    private IGrabbable _currentGrabbable;

    IControllable _currentLever;
    bool _isControllingLever;


    void Awake()
    {
        _characterColliderResizer.InitDefault();
        _characterRotator.Initialize(_groundRaycast);
        GameManager.Instance.Player = this;

        EventManager.Subscribe(EventType.OnFalled, HandleFallDeath);
        EventManager.Subscribe(EventType.OnFinishOxygen, InstantKill);
        SaveManager.Instance.Register(this);
    }

    void Start()
    {
        //ChangeMovement(_movements[0]);
    }

    void Update()
    {
        if (!_isAlive) return;

        _fallDamage.Tick(_isGround, _isSwimming, _isSliding, _rb.linearVelocity.y);

        _isGround = _groundRaycast.IsRaycasting(Vector3.down);
        _isSliding = _groundRaycast.IsSlopeTooSteep;
        _isGrab = _interactRaycast.IsRaycasting(_characterRotator.Mesh.forward);
        _isOnAir = !_isGround && !_isSwimming;
        _animationController.SetBool(AnimParams.Air, _isOnAir);
        _animationController.SetBool(AnimParams.IsFalling, _isFalling);

        _isFalling = _rb.linearVelocity.y < -0.5f && !_isGround && !_isSwimming;

        _rb.maxLinearVelocity = _groundRaycast.IsRaycasting(-Vector3.up) ? 15f : float.MaxValue;

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

        if (_isControllingLever)
        {
            UpdateLever();
            TryStopLever();
            return;
        }

        TryStartLever();

        if (_inputController.IsInteracting)
        {
            bool isPushableInFront = _interactRaycast.TryGetHit(out IPushable _);

            if (!isPushableInFront && _interactRaycast.TryGetHit(out IPresseable interactable))
            {
                Pressing();
            }

            _inputController.ResetInteract();
        }

        if (_inputController.IsJumping && _isGround && !_isCrouching && !_isSwimming)
        {
            _animationController.SetTrigger(AnimParams.Jump);

            if (_movement.CurrentSpeed > 0.1f)
            {
                _movement.Jump();
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

        _isSprinting = _inputController.IsSprinting && !_isCrouching;
        _isWalking = _inputController.IsWalking && !_isCrouching && !_isSprinting;

        if (_isPushingNow)
        {
            float xInput = _inputController.Direction.x;
            float zInput = _inputController.Direction.z;

            _animationController.SetFloat(AnimParams.XInput, xInput);
            _animationController.SetFloat(AnimParams.ZInput, zInput);
        }

        _animationController.SetFloat(AnimParams.Speed, _movement.CurrentSpeed);

        if (_inputController.Direction.sqrMagnitude > 0.1f * 0.1f)
            _animationController.SetBool(AnimParams.Move, true);
        else
            _animationController.SetBool(AnimParams.Move, false);

        TryStartGrab();
        TryStopGrab();
    }

    void TryStartLever()
    {
        if (_isControllingLever) return;
        if (!_inputController.IsLeverHolding || !_isGrab) return;

        if (_interactRaycast.TryGetHit(out IControllable lever))
        {
            _currentLever = lever;
            _isControllingLever = true;

            _inputController.EnableLeverMap();
            _oneHandGrabRig.Activate(lever.HandAnchor);
        }
    }

    void TryStopLever()
    {
        if (_inputController.IsLeverHolding) return;
        Debug.Log("lever try stop");
        _currentLever?.OnRelease();

        _oneHandGrabRig.Deactivate();
        _inputController.EnableMovementMap();

        _currentLever = null;
        _isControllingLever = false;
    }

    void UpdateLever()
    {
        float value = _currentLever.GetAxisValue(_inputController.LeverAxis);
        _currentLever.SetInputValue(value);
    }


    void FixedUpdate()
    {
        if (!_isAlive) return;

        Vector3 swimDir = _isSwimming ? GetSwimDirection() : Vector3.zero;

        //_externalVelocity = _externalVelocityProvider != null ? _externalVelocityProvider.ExternalVelocity : Vector3.zero;

        //UpdateInputMap();

        /*if (_isSwimming)
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
        }*/

        if (_isSwimming)
        {
            _movement.Swimming(swimDir);
        }
        else if (_isPushingNow)
        {
            _movement.Push(_inputController.Direction, _currentBox);
        }
        else if (_isSliding)
        {
            ChangePhysicMaterial();
            _movement.Slide(_inputController.Direction);
        }
        else if (_isCrouching)
        {
            _movement.Crouch(_inputController.Direction);
        }
        else if (_isSprinting)
        {
            _movement.Sprint(_inputController.Direction);
        }
        else if(_isWalking)
        {
            _movement.Walk(_inputController.Direction);
        }
        else
        {
            ResetPhysicsMaterial();
            _movement.Run(_inputController.Direction);
        }


        TryCrouching();
        SlideCharacter();
        UpdateCollider();

        //Vector3 dir = GetCurrentDirection();

        //_currentMovement.Advance(dir, _externalVelocity);

        if (!_isPushingNow)
        {
            if (_isSwimming)
            {
                _characterRotator.RotateSwimming(swimDir);
            }
            else
            {
                _characterRotator.Rotate(_inputController.Direction, _rb.linearVelocity);
            }
        }
    }

    void OnDestroy()
    {
        EventManager.Unsubscribe(EventType.OnFalled, HandleFallDeath);
        EventManager.Unsubscribe(EventType.OnFinishOxygen, InstantKill);

        if (SaveManager.Instance != null)
            SaveManager.Instance.Unregister(this);
    }

    /*void UpdateInputMap()
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
    }*/

    Vector3 GetSwimDirection()
    {
        Vector3 swimDir = _inputController.SwimDirection;

        if (_headPoint.position.y >= _currentWaterZone.BoundY && swimDir.y > 0f)
        {
            swimDir.y = 0f;
        }

        return swimDir;
    }

    /*Vector3 GetCurrentDirection()
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
    }*/

    void UpdateCollider()
    {
        if (_isCrouching)
        {
            _characterColliderResizer.SetSize(1f, new Vector3(0, 0.5f, 0));
        }
        else if(_isSliding)
        {
            _characterColliderResizer.SetSize(1f, new Vector3(0, 0.5f, 0)); //change values correctly
        }
        else
        {
            _characterColliderResizer.ResetSize();
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

    /*void ChangeMovement(MovementAdvance newMovement)
    {
        if (_currentMovement == newMovement) return;

        float prevSpeed = _currentMovement != null
        ? _currentMovement.CurrentSpeed
        : 0f;

        _currentMovement = newMovement;
        _currentMovement.Initialize(_rb);
        _currentMovement.SetSpeed(prevSpeed);

        _characterRotator.Initialize(_groundRaycast);
        }

    public void SetExternalVelocity(IExternalVelocity velocity)
    {
        _externalVelocityProvider = velocity;
    }*/

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

        //ChangeMovement(_movements[3]);
        
        _view.PlayBubbleVFX(true);
    }

    public void ExitWater()
    {
        _isSwimming = false;
        _rb.useGravity = true;
        _animationController.SetTrigger(AnimParams.StopSwimm);

        //ChangeMovement(_movements[0]);

        _view.PlayBubbleVFX(false);
    }

    public void Pressing()
    {
        if (_isPressingNow) return;

        _isPressingNow = true;

        if (_interactRaycast.TryGetHit(out IPresseable interactable))
        {
            interactable.Interact();

            if (interactable is ILeverHandRig handRig)
            {
                StartCoroutine(PressHandRoutine(handRig));
            }
        }

        StartCoroutine(ResetPress());
    }

    void TryStartGrab()
    {
        if (_currentGrabbable != null) return;
        if (!_inputController.IsPushing || !_isGrab) return;

        if (_interactRaycast.TryGetHit(out IGrabbable grabbable))
        {
            _currentGrabbable = grabbable;
            _handsGrabRig.Activate(grabbable);

            if (grabbable is IPushable pushable)
            {
                pushable.Pushing(_interactRaycast);
            }
        }
    }

    void TryStopGrab()
    {
        if (_currentGrabbable == null) return;

        if (!_inputController.IsPushing || !_isGrab)
        {
            if (_currentGrabbable is IPushable pushable)
            {
                pushable.StopPushing();
            }

            _handsGrabRig.Deactivate();
            _currentGrabbable = null;
        }
    }

    public void StartPush(PushableBox box)
    {
        if (_isPushingNow) return;

        _isPushingNow = true;
        _currentBox = box;

        Vector3 dir = (box.transform.position - transform.position).normalized;
        dir.y = 0f;
        _characterRotator.Mesh.forward = dir;
        _animationController.SetLayerWeight(1f);
    }

    public void StopPush()
    {
        if (!_isPushingNow) return;

        _isPushingNow = false;
        _currentBox = null;
        //_currentPushable = null;

        _animationController.SetLayerWeight(0f);
    }

    IEnumerator ResetPress()
    {
        yield return new WaitForSeconds(2f);
        _isPressingNow = false;
    }

    IEnumerator PressHandRoutine(ILeverHandRig handRig)
    {
        _oneHandGrabRig.Activate(handRig.RightHandPoint);
        yield return new WaitForSeconds(0.4f);
        _oneHandGrabRig.Deactivate();
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
        _movement.Jump();
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