using UnityEngine;

public class MusicRequester : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(_musicClip);
    }
}