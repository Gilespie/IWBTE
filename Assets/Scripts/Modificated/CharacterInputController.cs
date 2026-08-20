using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputController : MonoBehaviour
{
    InputsActions _inputActions;

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

    Vector3 _direction;
    public Vector3 Direction => new Vector3(_direction.x, 0f, _direction.y);

    Vector2 _uiDirection;
    public Vector2 UIDirection => _uiDirection;

    float _slideMoveAtm;
    public float SlideAxis => _slideMoveAtm;

    Vector3 _swimAtm;
    public Vector3 SwimDirection => _swimAtm;

    public bool IsPressedAny => Input.anyKey;

    void Awake()
    {
        _inputActions = SaveManager.Instance.InputActions;
        ScreenManager.Instance.SetInputController(this);
        EnableMovementMap();
    }

    void OnEnable()
    {
        _inputActions.PlayerMovement.Jump.started += HandleJump;
        _inputActions.PlayerMovement.Interact.started += HandleInteract;
        _inputActions.PlayerMovement.Crouch.started += HandleCrouch;
        _inputActions.PlayerMovement.Crouch.canceled += HandleUncrouch;
        _inputActions.PlayerMovement.Sprint.started += HandleSprint;
        _inputActions.PlayerMovement.Sprint.canceled += HandleUnsprint;
        _inputActions.PlayerMovement.Interact.started += HandlePush;
        _inputActions.PlayerMovement.Interact.canceled += HandleUnpush;
        _inputActions.PlayerMovement.Pause.started += HandlePause;
        _inputActions.PlayerMovement.AnyKey.started += HandleAnyKey;

        _inputActions.UI.Pause.started += HandleUIPause;
        _inputActions.UI.Apply.started += HandleApply;
    }

    void Update()
    {
        _direction = _inputActions.PlayerMovement.Move.ReadValue<Vector2>();
        _uiDirection = _inputActions.UI.UIMove.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        _inputActions.PlayerMovement.Jump.started -= HandleJump;
        _inputActions.PlayerMovement.Interact.started -= HandleInteract;
        _inputActions.PlayerMovement.Crouch.started -= HandleCrouch;
        _inputActions.PlayerMovement.Crouch.canceled -= HandleUncrouch;
        _inputActions.PlayerMovement.Sprint.started -= HandleSprint;
        _inputActions.PlayerMovement.Sprint.canceled -= HandleUnsprint;
        _inputActions.PlayerMovement.Interact.started -= HandlePush;
        _inputActions.PlayerMovement.Interact.canceled -= HandleUnpush;
        _inputActions.PlayerMovement.Pause.started -= HandlePause;
        _inputActions.PlayerMovement.AnyKey.started -= HandleAnyKey;

        _inputActions.UI.Pause.started -= HandleUIPause;
        _inputActions.UI.Apply.started -= HandleApply;

        DisableAllInput();
    }

    public void EnableMovementMap()
    {
        _inputActions.PlayerMovement.Enable();
        _inputActions.UI.Disable();
    }

    public void EnableUIMap()
    {
        _inputActions.PlayerMovement.Disable();
        _inputActions.UI.Enable();
    }

    public void DisableAllInput()
    {
        _inputActions.PlayerMovement.Disable();
        _inputActions.UI.Disable();

        _direction = Vector2.zero;
    }

    public void ResetJump()
    {
        _isJumping = false;
    }

    public void ResetInteract()
    {
        _isInteracting = false;
    }

    void HandlePause(InputAction.CallbackContext context)
    {
        EventManager.Trigger(EventType.OnPaused);
    }

    void HandleUIPause(InputAction.CallbackContext context)
    {
        EventManager.Trigger(EventType.OnPaused);
    }

    void HandleJump(InputAction.CallbackContext context)
    {
        _isJumping = true;
    }

    void HandleSprint(InputAction.CallbackContext context)
    {
        _isSprinting = true;
    }

    void HandleUnsprint(InputAction.CallbackContext context)
    {
        _isSprinting = false;
    }

    void HandleCrouch(InputAction.CallbackContext context)
    {
        _isCrouching = true;
    }

    void HandleUncrouch(InputAction.CallbackContext context)
    {
        _isCrouching = false;
    }

    void HandleInteract(InputAction.CallbackContext context)
    {
        _isInteracting = true;
    }

    void HandlePush(InputAction.CallbackContext context)
    {
        _isPushing = true;
    }

    void HandleUnpush(InputAction.CallbackContext context)
    {
        _isPushing = false;
    }

    void HandleApply(InputAction.CallbackContext context)
    {
        _isInteracting = true;
    }

    void HandleAnyKey(InputAction.CallbackContext context)
    {
        EventManager.Trigger(EventType.OnStartGame);
        Debug.Log("Started");
    }
}