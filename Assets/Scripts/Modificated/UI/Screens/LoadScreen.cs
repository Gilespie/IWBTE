using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadScreen : ScreenBase
{
    [SerializeField] ChapterData[] _chapters;
    [SerializeField] ChapterButton _buttonPrefab;
    [SerializeField] Transform _container;

    [Header("Preview")]
    [SerializeField] Image _previewImage;
    readonly List<ChapterButton> _buttons = new();
    GameObject _lastSelected;

    void Awake()
    {
        for (int i = 0; i < _chapters.Length; i++)
        {
            ChapterButton btn = Instantiate(_buttonPrefab, _container);
            btn.Setup(_chapters[i], SelectChapter, ShowPreview);
            _buttons.Add(btn);

            if (i == 0) _defaultButton = btn.Button;
        }

        if (_chapters.Length > 0)
            ShowPreview(_chapters[0]);
    }

    void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null || selected == _lastSelected) return;

        _lastSelected = selected;

        foreach (ChapterButton btn in _buttons)
        {
            if (selected == btn.Button.gameObject ||
                selected.transform.IsChildOf(btn.Button.transform))
            {
                ShowPreview(btn.Data);
                break;
            }
        }
    }

    void ShowPreview(ChapterData chapter)
    {
        if (_previewImage != null)
            _previewImage.sprite = chapter.Preview;
    }

    void SelectChapter(ChapterData chapter)
    {
        ScreenManager.Instance.DeactivateAll();

        SaveManager.Instance.StartChapter(chapter.IsStart, chapter.SpawnPosition);

        EventManager.Trigger(EventType.OnSceneTransition, chapter.SceneName);
    }
}