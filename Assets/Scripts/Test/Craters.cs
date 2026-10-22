using UnityEngine;

public class Craters : MonoBehaviour
{
    [SerializeField] GameObject _craterPrefab;
    [SerializeField] bool _isEventTrigger = true;

    private void Awake()
    {
        _craterPrefab.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<Rocket>(out _)) return;

        _craterPrefab.SetActive(true);

        if(_isEventTrigger) EventManager.Trigger(EventType.OnExplosion);
    }
}