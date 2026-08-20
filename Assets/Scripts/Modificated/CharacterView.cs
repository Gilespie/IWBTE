using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] ParticleSystem _bloodVFX;
    [SerializeField] ParticleSystem[] _bubbleVFX;
    [SerializeField] ParticleSystem _dustVFX;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _voices;

    public void PlayBloodVFX()
    {
        _bloodVFX.Play();
    }

    public void PlayVoice()
    {
        int index = Random.Range(0, _voices.Length);
        _audioSource.PlayOneShot(_voices[index]);
    }

    public void PlayBubbleVFX(bool value)
    {
        if(value)
        {
            foreach (var particle in _bubbleVFX)
            {
                particle.Play();
            }
        }
        else
        {
            foreach (var particle in _bubbleVFX)
            {
                particle.Stop();
            }
        }
    }

    public void PlayDustVFX(bool value)
    {
        if (value)
        {
            _dustVFX.Play();
        }
        else
        {
            _dustVFX.Stop();
        }

    }
}