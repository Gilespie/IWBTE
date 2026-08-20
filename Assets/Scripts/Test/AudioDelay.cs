using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _delayTime = 1f;

    private void Start()
    {
        Invoke(nameof(PlayAudio), _delayTime);
    }

    private void PlayAudio()
    {
        _audioSource.Play();
    }
}