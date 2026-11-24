using UnityEngine;

public class SoundCollision : MonoBehaviour
{
    [SerializeField] AudioClip[] _audios;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _volume = 0.01f;
    int m_IndexRandom;

    void Start()
    {
        m_IndexRandom = Random.Range(0, _audios.Length);
    }

    void OnCollisionEnter(Collision collision)
    {
        _audioSource.volume = collision.impulse.magnitude * _volume;
        _audioSource.PlayOneShot(_audios[m_IndexRandom]);
    }
}