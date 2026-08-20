using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : ScreenBase
{
    [Header("Panels")]
    [SerializeField] GameObject _graphicsPanel;
    [SerializeField] GameObject _keyboardPanel;
    [SerializeField] GameObject _volumePanel;
    [SerializeField] GameObject _creditsPanel;

    [Header("Buttons")]
    [SerializeField] Button _graphicBTN;
    [SerializeField] Button _keyboardBTN;
    [SerializeField] Button _volumeBTN;
    [SerializeField] Button _creditsBTN;
    [SerializeField] Button _backBTN;

    void Awake()
    {
        _graphicBTN.onClick.AddListener(() => ScreenManager.Instance.ActivateScreen(_graphicsPanel));
        _keyboardBTN.onClick.AddListener(() => ScreenManager.Instance.ActivateScreen(_keyboardPanel));
        _volumeBTN.onClick.AddListener(() => ScreenManager.Instance.ActivateScreen(_volumePanel));
        _creditsBTN.onClick.AddListener(() => ScreenManager.Instance.ActivateScreen(_creditsPanel));
        _backBTN.onClick.AddListener(()=> ScreenManager.Instance.DeactivateScreen());
    }
}