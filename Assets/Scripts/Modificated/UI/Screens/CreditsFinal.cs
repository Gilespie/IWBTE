using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsFinal : MonoBehaviour
{
    [SerializeField] EventType _eventType = EventType.OnSceneTransition;
    [SerializeField] string _levelName = "";
    [SerializeField] float _inputDelay = 1f;

    bool _isClicked;
    float _enableTime;
    InputsActions _inputActions;

    void Awake()
    {
        _inputActions = new InputsActions();
        _inputActions.Global.Enable();
        _inputActions.Global.AnyKey.performed += OnContinuePerformed;

        _enableTime = Time.unscaledTime + _inputDelay;
    }

    void OnDestroy()
    {
        _inputActions.Global.AnyKey.performed -= OnContinuePerformed;
        _inputActions.Global.Disable();
        _inputActions.Dispose();
    }

    void OnContinuePerformed(InputAction.CallbackContext ctx)
    {
        if (_isClicked) return;
        if (Time.unscaledTime < _enableTime) return;

        _isClicked = true;

        SaveManager.Instance.DeleteGameSave();
        EventManager.Trigger(_eventType, _levelName);
    }
}