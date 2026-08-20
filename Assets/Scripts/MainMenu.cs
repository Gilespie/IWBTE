using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string _startLevelName = "";
    [SerializeField] GameObject _mainMenuPanel;
    [SerializeField] GameObject _settingsPanel;
    //[SerializeField] GameObject _controlsPanel;
    [SerializeField] GameObject _creditsPanel;
    [SerializeField] Button _playButton;
    [SerializeField] Button _settingsButton;
    //[SerializeField] Button _controlButton;
    [SerializeField] Button _creditsButton;
    [SerializeField] Button _exitButton;

    void Start()
    {
        //_playButton.onClick.AddListener(() => LoadSceneManager.Instance.LoadScene(_startLevelName));
        _settingsButton.onClick.AddListener(ShowSettings);
        //_controlButton.onClick.AddListener(ShowControls);
        _creditsButton.onClick.AddListener(ShowCredits);
        _exitButton.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void ShowCredits()
    {
        _mainMenuPanel.SetActive(false);
        _creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        _creditsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    /*public void ShowControls()
    {
        _mainMenuPanel.SetActive(false);
        _controlsPanel.SetActive(true);
    }*/

    /*public void CloseControls()
    {
        _controlsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }*/

    public void ShowSettings()
    {
        _mainMenuPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        _mainMenuPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }
}