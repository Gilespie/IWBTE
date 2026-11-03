using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Collider _collider;

    [SerializeField] int _chapterIndex = 1;

    [SerializeField] Transform _spawnPoint;

    IEnumerator Start()
    {
        if (SaveManager.PendingChapter != _chapterIndex)
            yield break;

        yield return null; 

        SaveManager.PendingChapter = -1;

        Character player = GameManager.Instance.Player;
        if (player == null) yield break;

        Transform point = _spawnPoint != null ? _spawnPoint : transform;

        if (player.TryGetComponent(out Rigidbody rb))
        {
            rb.position = point.position;
            rb.linearVelocity = Vector3.zero;
        }

        player.transform.position = point.position;

        if (GameManager.Instance.CameraPointFollow != null)
            GameManager.Instance.CameraPointFollow.SnapToTarget();

        SaveManager.Instance.SaveGame();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Character>()) return;

        SaveManager.Instance.UnlockChapter(_chapterIndex);
        EventManager.Trigger(EventType.OnCheckpoint, _chapterIndex);
        _collider.enabled = false;
    }
}