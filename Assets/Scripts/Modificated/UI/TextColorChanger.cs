using UnityEngine;
using UnityEngine.EventSystems;

public class TextColorChanger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] TMPro.TextMeshProUGUI _text;
    [SerializeField] private Color _normalColor = new Color(0.08503918f, 0.4622642f, 0.3591022f, 1f);
    [SerializeField] private Color _hoverColor = new Color(0f, 1f, 0.6631906f, 1f);

    void Awake()
    {
        if(_text == null) _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if(_text != null) _text.color = _normalColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _text.color = _normalColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _text.color = _hoverColor;
    }
}