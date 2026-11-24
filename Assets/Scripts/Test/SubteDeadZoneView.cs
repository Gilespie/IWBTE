using UnityEngine;
using UnityEngine.VFX;

public class SubteDeadZoneView : MonoBehaviour
{
    [SerializeField] SubteDeadZone _deadZone;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] VisualEffect _electricityVfx;
    [SerializeField] AudioClip _clip;

    private void Awake()
    {
        _deadZone.OnEntered += HandleEntered;
    }

    private void OnDestroy()
    {
        _deadZone.OnEntered -= HandleEntered;
    }

    private void HandleEntered()
    {
        _electricityVfx.Play();

        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.PlayOneShot(_clip);
    }
}