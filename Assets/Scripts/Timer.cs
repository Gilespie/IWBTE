using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _maxTimer = 30f;
    private float _currentTime = 0f;
    public float CurrentTime => _currentTime;
    private bool _isStarted = false;
    public bool IsStarted => _isStarted;
    private bool _isDone = false;

    private void Start()
    {
        _currentTime = _maxTimer;
    }

    private void Update()
    {
        if (!_isStarted || _isDone) return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            _isStarted = false;
            _currentTime = 0f;
            EventManager.Trigger(EventType.OnTimeOut);
            _isDone = true;
        }
    }

    public void StartTimer()
    {
        _isStarted = true;
    }
}