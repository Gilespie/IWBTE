using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCheats : SingletonBase<DebugCheats>
{
    [SerializeField] bool _isDebugMode = false;
    [SerializeField] Transform[] _subdivisions;
    [SerializeField] private Transform _target;

/*    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            _isDebugMode = !_isDebugMode;
            Debug.Log($"Debug mode: {_isDebugMode}");
        }

        if(_isDebugMode)
        {
            for (int i = 0; i < _subdivisions.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    _target.position = _subdivisions[i].position;
                }
            }
        }
    }*/

    static readonly Key[] _digitKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0
    };

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard[Key.F9].wasPressedThisFrame)
        {
            _isDebugMode = !_isDebugMode;
            Debug.Log($"Debug mode: {_isDebugMode}");
        }

        if (!_isDebugMode) return;

        int count = Mathf.Min(_subdivisions.Length, _digitKeys.Length);
        for (int i = 0; i < count; i++)
        {
            if (keyboard[_digitKeys[i]].wasPressedThisFrame)
            {
                _target.position = _subdivisions[i].position;
            }
        }
    }
}