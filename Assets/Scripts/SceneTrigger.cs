using UnityEngine;

public class SceneTrigger : Trigger
{
    [SerializeField] EventType _eventType;
    [SerializeField] string _levelName = "";

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        EventManager.Trigger(_eventType, _levelName);
    }
}