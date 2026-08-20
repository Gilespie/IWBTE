using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SaveManager : SingletonBase<SaveManager>
{
    InputsActions _inputActions;
    public InputsActions InputActions => _inputActions;

    private SaveSlot<SettingsData> _settings;
    private SaveSlot<SaveGameData> _game;

    public SettingsData Settings => _settings.Data;

    private readonly List<ISaveable> _saveables = new();

    protected override void Awake()
    {
        base.Awake();

        _settings = new SaveSlot<SettingsData>("settings.json");
        _game = new SaveSlot<SaveGameData>("save.json");

        _inputActions = new InputsActions();

        LoadSettings();
        Apply();
        ApplyInputBindings();
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.OnCheckpoint, HandleCheckpoint);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnCheckpoint, HandleCheckpoint);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
            LoadGame();
    }

    private void HandleCheckpoint(params object[] args)
    {
        SaveGame();
    }

    public void Register(ISaveable s) => _saveables.Add(s);
    public void Unregister(ISaveable s) => _saveables.Remove(s);

    public void SaveSettings()
    {
        SaveInputBindings();
        _settings.Save();
    }

    public void LoadSettings()
    {
        if (!_settings.Exists)
        {
            _settings.Save();
            return;
        }
        _settings.Load();
    }

    public void Apply()
    {
        QualitySettings.SetQualityLevel(Settings.QualityIndex);
        QualitySettings.vSyncCount = Settings.VSync;
        Screen.fullScreen = Settings.Fullscreen;
    }

    public void SaveInputBindings() =>
        Settings.InputBindingsOverridesJson = _inputActions.asset.SaveBindingOverridesAsJson();

    public void ApplyInputBindings()
    {
        if (!string.IsNullOrEmpty(Settings.InputBindingsOverridesJson))
            _inputActions.asset.LoadBindingOverridesFromJson(Settings.InputBindingsOverridesJson);
    }

    public void SaveGame()
    {
        foreach (var s in _saveables)
            s.CaptureState(_game.Data);

        _game.Save();
    }

    public void LoadGame()
    {
        if (!_game.Exists)
            return;

        _game.Load();

        foreach (var s in _saveables)
            s.RestoreState(_game.Data);
    }
}