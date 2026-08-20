using UnityEngine;
using UnityEngine.VFX;

public class Birds : MonoBehaviour
{
    [SerializeField] float _delayToActivate;
    [SerializeField] AudioClip _clip;
    VisualEffect _vfx;
    AudioSource _audioSource;

    void OnEnable()
    {
        RocketExplosion.OnExplosed += OnBirdsStart;
    }

    void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _audioSource.clip = _clip;
        _vfx = GetComponent<VisualEffect>();
        _vfx.Stop();
    }

    void OnDisable()
    {
        RocketExplosion.OnExplosed -= OnBirdsStart;
    }

    public void OnBirdsStart()
    {
        Invoke(nameof(BirdFly), _delayToActivate);
    }

    void BirdFly()
    {
        _audioSource.Play();
        _vfx.Play();
    }
}