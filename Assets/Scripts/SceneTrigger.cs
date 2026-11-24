using UnityEngine;
using UnityEngine.TextCore.Text;

public class SceneTrigger : Trigger
{
    [SerializeField] EventType _eventType;
    [SerializeField] string _levelName = "";

    [Header("Spawn in the new scene")]
    [SerializeField] bool _overrideSpawn = true;
    [SerializeField] Transform _spawnPosition;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Character>(out var character) || !character.IsAlive) return;

        base.OnTriggerEnter(other);

        if (_overrideSpawn)
            SaveManager.Instance.SetSpawnPosition(_spawnPosition.position, _levelName);

        EventManager.Trigger(_eventType, _levelName);
    }
}