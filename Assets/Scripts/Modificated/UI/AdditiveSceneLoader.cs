using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
    [SerializeField] private string _loadLevelName = "";
    [SerializeField] private string _unloadLevelName = "";
    private bool _isLoaded;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            if(!_isLoaded)
            {
                StartCoroutine(LoadAdditiveSceneRoutine(_loadLevelName));
            }
            else
            {
                StartCoroutine(UnloadAdditiveSceneRoutine(_unloadLevelName));
            }
        }
    }

    private IEnumerator LoadAdditiveSceneRoutine(string name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(_loadLevelName, LoadSceneMode.Additive);

        while(!async.isDone)
        {
            yield return null;
        }

        _isLoaded = !_isLoaded;
    }

    private IEnumerator UnloadAdditiveSceneRoutine(string name)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(_unloadLevelName);

        while (!async.isDone)
        {
            yield return null;
        }

        _isLoaded = !_isLoaded;
    }
}