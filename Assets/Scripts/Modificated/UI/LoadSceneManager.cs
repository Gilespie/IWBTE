using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : SingletonBase<LoadSceneManager>
{
    [SerializeField] float _minLoadingTime = 3f;
    [SerializeField] LoadProgressUI _loaderUI;
    [SerializeField] ScreenFader _fader;
    bool _isLoading = false;

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnDead, RestartGame);
        EventManager.Subscribe(EventType.OnSceneTransition, LoadScene);
    }

    void Start()
    {
        _fader.FadeIn();
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnDead, RestartGame);
        EventManager.Unsubscribe(EventType.OnSceneTransition, LoadScene);
    }

    public void LoadScene(params object[] args)
    {
        string sceneName = (string)args[0];

        if (!_isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    public void RestartGame(params object[] args)
    {
        string currentLevelname = SceneManager.GetActiveScene().name;

        LoadScene(currentLevelname);
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        _isLoading = true;

        yield return _fader.FadeOut();

        _loaderUI.ShowText();

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (asyncOp.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < _minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
            yield return null;

        _loaderUI.HideText();

        yield return _fader.FadeIn();

        _isLoading = false;
    }
}