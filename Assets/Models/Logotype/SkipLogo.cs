using UnityEngine;
using UnityEngine.InputSystem;

public class SkipLogo : MonoBehaviour
{
    [SerializeField] KeyCode _skipLogoKey;
    [SerializeField] string _levelName;
    bool _isSkipped = false;
    InputsActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputsActions();
        _inputActions.Enable();
        _inputActions.Logo.AnyKey.performed += OnSkipLogoPerformed;
    }

    private void OnDestroy()
    {
        _inputActions.Logo.AnyKey.performed -= OnSkipLogoPerformed;
        _inputActions.Disable();
        _inputActions.Dispose();
    }

    void OnSkipLogoPerformed(InputAction.CallbackContext ctx)
    {
        if (_isSkipped) return;

        EventManager.Trigger(EventType.OnSceneTransition, _levelName);
        _isSkipped = true;
    }
}