using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonBase<PauseManager>
{
    List<MonoBehaviour> _owners = new List<MonoBehaviour>();
    bool _isPaused = false;
    public bool IsPaused => _isPaused;

    public void Pause(MonoBehaviour mono)
    {
        Time.timeScale = 0f;

        if (_owners.Contains(mono)) return;
        
        _owners.Add(mono);
        _isPaused = true;
    }

    public void Unpause(MonoBehaviour mono)
    {
        if(_owners.Contains(mono))
            _owners.Remove(mono);

        if(_owners.Count <= 0)
        {
            Time.timeScale = 1f;
            _isPaused = false;
        }    
    }
}