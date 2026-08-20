using UnityEngine;
using UnityEngine.VFX;

public class FireworksController : MonoBehaviour
{
    [SerializeField] VisualEffect[] _effects;
    [SerializeField] AudioSource[] _source;

    public void StartEvent()
    {
        PlayVFX();
        Invoke(nameof(PlaySound), 3f);
        _source[1].Play();
    }

    void PlaySound()
    {
        _source[0].Play();
        _source[2].Play();
    }

    void PlayVFX()
    {
        foreach (var effect in _effects)
        {
            effect.Play();
        }
    }

    public void StopPlay()
    {
        foreach (var effect in _effects)
        {
            effect.Stop();
        }

        _source[0].Stop();
    }
}