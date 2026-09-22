using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputController : MonoBehaviour
{
    InputsActions _inputActions;

    bool _isWalking;
    public bool IsWalking => _isWalking;
    bool _isCrouching;
    public bool IsCrouching => _isCrouching;

    bool _isJumping;
    public bool IsJumping => _isJumping;

    bool _isSprinting;
    public bool IsSprinting => _isSprinting;

    bool _isInteracting;
    public bool IsInteracting => _isInteracting;

    bool _isPushing;
    public bool IsPushing => _isPushing;

    bool _isClimbing;
    public bool IsClimbing => _isClimbing;

    Vector3 _direction;
    public Vector3 Direction => new Vector3(_direction.x, 0f, _direction.y);

    Vector2 _uiDirection;
    public Vector2 UIDirection => _uiDirection;

    float _slideMoveAtm;
    public float SlideAxis => _slideMoveAtm;

    Vector3 _swimDirection;
    public Vector3 SwimDirection => _swimDirection;

    Vector2 _leverInput;
    public Vector2 LeverAxis => _leverInput;

    bool _isLeverHolding;
    public bool IsLeverHolding => _isLeverHolding;

    void Awake()
    {
        _inputActions = SaveManager.Instance.InputActions;
        ScreenManager.Instance.SetInputController(this);
        _inputActions.Global.Enable();
        EnableMovementMap();
    }

    void OnEnable()
    {
        _inputActions.PlayerMovement.Jump.started += HandleJump;
        _inputActions.PlayerMovement.Crouch.started += HandleCrouch;
        _inputActions.PlayerMovement.Crouch.canceled += HandleUncrouch;
        _inputActions.PlayerMovement.Sprint.started += HandleSprint;
        _inputActions.PlayerMovement.Sprint.canceled += HandleUnsprint;
        _inputActions.PlayerMovement.Walk.started += HandleWalk;
        _inputActions.PlayerMovement.Walk.canceled += HandleUnwalk;

        _inputActions.PlayerMovement.Interact.started += HandleInteractStarted;
        _inputActions.PlayerMovement.Interact.performed += HandleInteractPerformed;
        _inputActions.PlayerMovement.Interact.canceled += HandleInteractCanceled;

        _inputActions.Global.PauseCancel.started += HandlePause;
        _inputActions.Global.AnyKey.started += HandleAnyKey;
        _inputActions.Global.Submit.performed += HandleSubmit;
    }

    void Update()
    {
        _direction = _inputActions.PlayerMovement.Move.ReadValue<Vector2>();
        _swimDirection = _inputActions.PlayerMovement.Swim.ReadValue<Vector3>();
        _uiDirection = _inputActions.UI.UIMove.ReadValue<Vector2>();
        _leverInput = _inputActions.Lever.Movement.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        _inputActions.PlayerMovement.Jump.started -= HandleJump;
        _inputActions.PlayerMovement.Crouch.started -= HandleCrouch;
        _inputActions.PlayerMovement.Crouch.canceled -= HandleUncrouch;
        _inputActions.PlayerMovement.Sprint.started -= HandleSprint;
        _inputActions.PlayerMovement.Sprint.canceled -= HandleUnsprint;
        _inputActions.PlayerMovement.Walk.started -= HandleWalk;
        _inputActions.PlayerMovement.Walk.canceled -= HandleUnwalk;

        _inputActions.PlayerMovement.Interact.started -= HandleInteractStarted;
        _inputActions.PlayerMovement.Interact.performed -= HandleInteractPerformed;
        _inputActions.PlayerMovement.Interact.canceled -= HandleInteractCanceled;

        _inputActions.Global.PauseCancel.started -= HandlePause;
        _inputActions.Global.AnyKey.started -= HandleAnyKey;
        _inputActions.Global.Submit.performed -= HandleSubmit;

        DisableAllInput();
    }

    public void EnableMovementMap()
    {
        _inputActions.PlayerMovement.Enable();
        _inputActions.UI.Disable();
        _inputActions.Lever.Disable();
    }

    public void EnableUIMap()
    {
        _inputActions.PlayerMovement.Disable();
        _inputActions.Lever.Disable();
        _inputActions.UI.Enable();
    }

    public void EnableLeverMap()
    {
        _inputActions.PlayerMovement.Move.Disable();
        _inputActions.PlayerMovement.Jump.Disable();
        _inputActions.PlayerMovement.Crouch.Disable();
        _inputActions.PlayerMovement.Sprint.Disable();
        _inputActions.UI.Disable();
        _inputActions.Lever.Enable();
    }

    public void DisableAllInput()
    {
        _inputActions.PlayerMovement.Disable();
        _inputActions.UI.Disable();
        _inputActions.Lever.Disable();

        _direction = Vector2.zero;
    }

    public void ResetJump() => _isJumping = false;
    public void ResetInteract() => _isInteracting = false;
    public void ResetClimb() => _isClimbing = false;

    void HandlePause(InputAction.CallbackContext context) => EventManager.Trigger(EventType.OnPaused);
    void HandleJump(InputAction.CallbackContext context) => _isJumping = true;
    void HandleSprint(InputAction.CallbackContext context) => _isSprinting = true;
    void HandleUnsprint(InputAction.CallbackContext context) => _isSprinting = false;
    void HandleCrouch(InputAction.CallbackContext context) => _isCrouching = true;
    void HandleUncrouch(InputAction.CallbackContext context) => _isCrouching = false;
    void HandleWalk(InputAction.CallbackContext context) => _isWalking = true;
    void HandleUnwalk(InputAction.CallbackContext context) => _isWalking = false;

    void HandleSubmit(InputAction.CallbackContext context) { }
    void HandleAnyKey(InputAction.CallbackContext context) => EventManager.Trigger(EventType.OnStartGame);

    void HandleInteractStarted(InputAction.CallbackContext context)
    {
        _isClimbing = true;
        _isInteracting = true;
    }

    void HandleInteractPerformed(InputAction.CallbackContext context)
    {
        _isPushing = true;
        _isLeverHolding = true;
    }

    void HandleInteractCanceled(InputAction.CallbackContext context)
    {
        _isPushing = false;
        _isLeverHolding = false;
    }
}