using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractButton : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _event;
    [SerializeField] private InteractMode _interactMode = InteractMode.Once;
    [SerializeField, ColorUsage(true, true)] private Color _colorOn;
    [SerializeField, ColorUsage(true, true)] private Color _colorOff;
    [SerializeField] private AudioClip _sfxOn;
    [SerializeField] private AudioClip _sfxOff;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _resetDelay = 10f;
    private bool _isActive = false;
    private int _count = 0;

    private void Start()
    {
        ActivateLamp(_isActive);
    }

    [ContextMenu("Interact")]
    public void Interact()
    {
        switch (_interactMode)
        {
            case InteractMode.Once:
                if (_count > 0) return;
                ActivateButton();
                break;

            case InteractMode.Toggle:
                ActivateButton();
                break;
        }
    }

    private void ActivateButton()
    {
        _isActive = !_isActive;
        PlaySound(_sfxOn, _sfxOff);
        ActivateLamp(_isActive);
        _event?.Invoke();
        StartCoroutine(ResetRoutine());
        _count++;
    }

    private void PlaySound(AudioClip clip1, AudioClip clip2)
    {
        _audioSource.clip = _isActive ? clip1 : clip2;
        _audioSource.Play();
    }

    public void ResetSwitch()
    {
        _isActive = false;
        PlaySound(_sfxOn, _sfxOff);
        ActivateLamp(false);
    }

    private void ActivateLamp(bool isActive)
    {
        if (_isActive)
        {
            _meshRenderer.material.SetColor("_EmissionColor", _colorOn);
        }
        else
        {
            _meshRenderer.material.SetColor("_EmissionColor", _colorOff);
        }
    }

    IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(_resetDelay);
        ResetSwitch();
    }
}
