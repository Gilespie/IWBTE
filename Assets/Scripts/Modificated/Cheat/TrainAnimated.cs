using UnityEngine;
using UnityEngine.Events;

public class TrainAnimated : MonoBehaviour
{
    [SerializeField] Vector2 _waitingTime;
    [SerializeField] UnityEvent _events;
    float _timer;

    void Awake()
    {
        SetTimer();
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if(_timer <= 0)
        {
            _events?.Invoke();
            SetTimer();
        }
    }

    float SetTimer()
    {
        _timer = Random.Range(_waitingTime.x, _waitingTime.y);
        return _timer;
    }
}