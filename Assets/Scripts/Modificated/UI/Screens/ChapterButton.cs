using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour
{
    [SerializeField] Button _button;
    public Button Button => _button;
    ChapterData _data;
    public ChapterData Data => _data;
    Action<ChapterData> _onSelected;

    public void Setup(ChapterData data, Action<ChapterData> onClick, Action<ChapterData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        _button.onClick.AddListener(() => onClick(data));
    }

    public void OnSelect(BaseEventData eventData)
    {
        _onSelected?.Invoke(_data);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}