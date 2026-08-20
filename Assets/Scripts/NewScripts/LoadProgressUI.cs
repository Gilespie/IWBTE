using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadProgressUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _stateText;
    [SerializeField] Slider _sliderProgress;
    [SerializeField] string _loadingMessage = "Loading...";

    void Awake()
    {
        Hide();
    }

    public void Show()
    {
        _sliderProgress.value = 0f;
        _sliderProgress.gameObject.SetActive(true);

        _stateText.text = _loadingMessage;
        _stateText.enabled = true;
    }

    public void ShowText()
    {
        _stateText.text = _loadingMessage;
        _stateText.enabled = true;
    }

    public void SetProgress(float value)
    {
        _sliderProgress.value = value;
    }

    public void Hide()
    {
        _stateText.enabled = false;
        _sliderProgress.gameObject.SetActive(false);
    }

    public void HideText()
    {
        _stateText.enabled = false;
        _sliderProgress.gameObject.SetActive(false);
    }
}