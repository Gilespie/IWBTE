using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenBase : MonoBehaviour, IScreen
{
    [Header("Main settings")]
    [SerializeField] GameObject _root;
    [SerializeField] Button _defaultButton;

    public void Activate()
    {
        PauseManager.Instance.Pause(this);
        _root.SetActive(true);
        StartCoroutine(SelectDefaultButton());
    }

    public void Deactivate()
    {
        EventSystem.current.SetSelectedGameObject(null);
        PauseManager.Instance.Unpause(this);
        _root.SetActive(false);
    }

    IEnumerator SelectDefaultButton()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_defaultButton.gameObject);
    }
}
