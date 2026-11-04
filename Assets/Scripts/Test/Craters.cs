using UnityEngine;
using UnityEngine.Events;

public class Craters : MonoBehaviour
{
    [SerializeField] GameObject _craterPrefab;
    [SerializeField] UnityEvent _events;
    [SerializeField] bool _isEventTrigger = true;
    [SerializeField] bool _isEvents = true;

    void Awake()
    {
        _craterPrefab.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<Rocket>(out _)) return;

        _craterPrefab.SetActive(true);

        if(_isEventTrigger) EventManager.Trigger(EventType.OnExplosion);
        if(_isEvents) _events?.Invoke();
    }
}