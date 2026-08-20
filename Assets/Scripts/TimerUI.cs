using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Timer _timer;

    private void Start()
    {
        _timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_timer.IsStarted)
            UpdateTimerUI();
        else
            _timerText.gameObject.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        _timerText.gameObject.SetActive(true);

        float time = _timer.CurrentTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int fractions = Mathf.FloorToInt((time % 1f) * 100f);

        _timerText.text = $"{minutes:00}:{seconds:00}.{fractions:00}";
    }
}