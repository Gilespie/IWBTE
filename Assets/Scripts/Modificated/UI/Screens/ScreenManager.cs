using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : SingletonBase<ScreenManager>
{
    [SerializeField] CharacterInputController _characterInputController;

    List<IScreen> _screens = new List<IScreen>();

    public void SetInputController(CharacterInputController controller)
    {
        _characterInputController = controller;
    }

    public bool IfScreenActive(IScreen screen)
    {
        if (_screens.Count <= 0) return false;

        foreach (var item in _screens)
        {
            if (item == screen) return true;
        }

        return false;
    }

    public bool IfScreenActive(GameObject screen)//ejemplo con GameObject
    {
        var s = screen.GetComponent<IScreen>();

        if(s == null) return false;

        return IfScreenActive(s);
    }

    public void ActivateScreen(IScreen screen)
    {
        if (_screens.Count == 0)
        {
            _characterInputController.EnableUIMap();
        }

        if (_screens.Count > 0) _screens[_screens.Count - 1].Deactivate();

        _screens.Add(screen); 
        screen.Activate();
    }

    public void ActivateScreen(GameObject screen)//ejemplo con GameObject
    {
        var s = screen.GetComponent<IScreen>();

        if (s == null) return;

        ActivateScreen(s);
    }

    public void DeactivateScreen()
    {
        if (_screens.Count == 0) return;

        var last = _screens[_screens.Count - 1];
        last.Deactivate();
        _screens.RemoveAt(_screens.Count - 1);

        if(_screens.Count > 0)
        {
            _screens[_screens.Count - 1].Activate();
        }
        else
        {
            _characterInputController.EnableMovementMap();
        }
    }

    public void DeactivateAll()
    {
        if (_screens.Count == 0) return;

        foreach(var screen in _screens)
        {
            screen.Deactivate();
        }

        _screens.Clear();

        _characterInputController.EnableMovementMap();
    }
}