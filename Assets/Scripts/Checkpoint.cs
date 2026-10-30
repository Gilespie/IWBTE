using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Collider _collider;

    [SerializeField] int _chapterIndex = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Character>()) return;

        SaveManager.Instance.UnlockChapter(_chapterIndex);
        EventManager.Trigger(EventType.OnCheckpoint, _chapterIndex);
        _collider.enabled = false;
    }
}