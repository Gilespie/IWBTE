using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionSelector : MonoBehaviour, IMoveHandler
{
    public event Action<int> OnValueChanged;

    [SerializeField] TMP_Text _valueText;
    [SerializeField] List<string> _values;
    int _currentIndex;

    public void OnMove(AxisEventData eventData)
    {
        Debug.Log(eventData.moveDir);

        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
                Previous();
                eventData.Use();
                break;

            case MoveDirection.Right:
                Next();
                eventData.Use();
                break;
        }
    }

    public void Initialize(List<string> values, int startIndex = 0)
    {
        _values = values;
        _currentIndex = Mathf.Clamp(startIndex, 0, _values.Count - 1);

        UpdateText();
    }

    void Next()
    {
        _currentIndex++;

        if (_currentIndex >= _values.Count)
            _currentIndex = 0;

        UpdateText();
    }

    void Previous()
    {
        _currentIndex--;

        if (_currentIndex < 0)
            _currentIndex = _values.Count - 1;

        UpdateText();
    }

    void UpdateText()
    {
        _valueText.text = _values[_currentIndex];
        OnValueChanged?.Invoke(_currentIndex);
    }
}