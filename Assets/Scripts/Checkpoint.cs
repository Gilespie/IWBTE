using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Collider _collider;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            EventManager.Trigger(EventType.OnCheckpoint);
            _collider.enabled = false;
        }
    }
}