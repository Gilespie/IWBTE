using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindKey : MonoBehaviour
{
    [Header("Binding target")]
    [SerializeField] ControlSchemeType _controlSchemeType = ControlSchemeType.Keyboard;
    [SerializeField] ActionMap _actionMapName = ActionMap.PlayerMovement;
    [SerializeField] ActionName _actionName;
    [SerializeField] ActionCompositePart _compositePartName = ActionCompositePart.None;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _valueText;

    InputAction _action;
    InputActionRebindingExtensions.RebindingOperation _rebindOperation;
    bool _isRebinding = false;
    bool _waitingForKeyRelease = false;

    void OnEnable()
    {
        _action = SaveManager.Instance.InputActions.asset
            .FindActionMap(_actionMapName.ToString())
            .FindAction(_actionName.ToString());

        RefreshDisplay();
    }

    void Update()
    {
        if (_isRebinding) return;

        if (_waitingForKeyRelease)
        {
            if (!Keyboard.current.enterKey.isPressed && !Keyboard.current.eKey.isPressed)
                _waitingForKeyRelease = false;

            return;
        }

        if (!IsSelected()) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartRebind();
        }
    }

    int GetBindingIndex()
    {
        for(int i = 0; i < _action.bindings.Count; i++)
        {
            var binding = _action.bindings[i];

            bool groupMatches = binding.groups != null &&
                binding.groups.Contains(_controlSchemeType.ToString());

            bool partMatches = _compositePartName == ActionCompositePart.None
            || binding.name.Equals(_compositePartName.ToString(), System.StringComparison.OrdinalIgnoreCase);

            if (groupMatches && partMatches)
                return i;
        }

        return 0;
    }

    bool IsSelected()
    {
        return UnityEngine.EventSystems.EventSystem.current != null
            && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject;
    }

    public void StartRebind()
    {
        if (_isRebinding) return;

        _isRebinding = true;
        _valueText.text = "...";

        _action.Disable();

        int index = GetBindingIndex();

        _rebindOperation = _action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("Keyboard/enter")
            .WithControlsExcluding("Keyboard/e")
            .WithControlsExcluding("Keyboard/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(OnRebindComplete)
            .OnCancel(OnRebindCancel)
            .Start();
    }

    void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        _isRebinding = false;
        _waitingForKeyRelease = true;

        _action.Enable();

        RefreshDisplay();
        SaveManager.Instance.SaveSettings();

        operation.Dispose();
    }

    void OnRebindCancel(InputActionRebindingExtensions.RebindingOperation operation)
    {
        _isRebinding = false;
        _waitingForKeyRelease = true;

        _action.Enable();
        RefreshDisplay();

        operation.Dispose();
    }

    void RefreshDisplay()
    {
        int index = GetBindingIndex();

        _valueText.text = InputControlPath.ToHumanReadableString(
            _action.bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }

    public void ResetRebind()
    {
        _action.RemoveAllBindingOverrides();
        RefreshDisplay();        
    }
}