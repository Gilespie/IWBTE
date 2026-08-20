using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : ScreenBase
{
    [Header("Panels")]
    [SerializeField] GameObject _settingsPanel;

    [Header("Buttons")]
    [SerializeField] Button _continueBTN;
    [SerializeField] Button _restartBTN;
    [SerializeField] Button _loadBTN;
    [SerializeField] Button _settingsBTN;
    [SerializeField] Button _exitBTN;
    bool _isRestarting = false;

    void Awake()
    {
        _continueBTN.onClick.AddListener(() => OnPausePressed(false));
        _restartBTN.onClick.AddListener(RestartLevel);
        _loadBTN.onClick.AddListener(() => Debug.Log("Load Game clicked"));
        _settingsBTN.onClick.AddListener(() => ScreenManager.Instance.ActivateScreen(_settingsPanel));
        _exitBTN.onClick.AddListener(ExitFromGame);
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.OnPaused, OnPausePressed);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnPaused, OnPausePressed);
    }

    void OnPausePressed(params object[] args)
    {
        if (!ScreenManager.Instance.IfScreenActive(this))
        {
            ScreenManager.Instance.ActivateScreen(this);
            return;
        }

        ScreenManager.Instance.DeactivateScreen();
    }

    void RestartLevel()
    {
        if (_isRestarting) return;

        OnPausePressed();
        LoadSceneManager.Instance.RestartGame();
        _isRestarting = true;
    }

    void ExitFromGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}