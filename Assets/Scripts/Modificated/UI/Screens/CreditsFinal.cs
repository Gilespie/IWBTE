using UnityEngine;
using UnityEngine.UI;

public class CreditsFinal : MonoBehaviour
{
    [SerializeField] Button _continuedBTN;
    [SerializeField] EventType _eventType = EventType.OnSceneTransition;
    [SerializeField] string _levelName = "";

    bool _isClicked;

    void Start()
    {
        _continuedBTN.onClick.AddListener(OnContinueButtonClicked);
    }

    void OnDestroy()
    {
        if (_continuedBTN != null)
            _continuedBTN.onClick.RemoveListener(OnContinueButtonClicked);
    }

    void OnContinueButtonClicked()
    {
        if (_isClicked) return;

        _isClicked = true;

        SaveManager.Instance.DeleteGameSave();
        EventManager.Trigger(_eventType, _levelName);
    }
}