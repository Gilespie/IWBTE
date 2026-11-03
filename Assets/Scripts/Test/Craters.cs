using UnityEngine;

public class Craters : MonoBehaviour
{
    [SerializeField] GameObject _craterPrefab;
    [SerializeField] bool _isEventTrigger = true;

    void Awake()
    {
        _craterPrefab.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<Rocket>(out _)) return;

        _craterPrefab.SetActive(true);

        if(_isEventTrigger) EventManager.Trigger(EventType.OnExplosion);
    }
}