using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _event;
    [SerializeField] private InteractMode _interactMode = InteractMode.Once;
    [SerializeField] private Color _colorOn;
    [SerializeField] private Color _colorOff;
    [SerializeField] private AudioClip _sfxOn;
    [SerializeField] private AudioClip _sfxOff;
    [SerializeField] private Light _light;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _resetDelay = 10f;
    private Animator _animator;
    private bool _isActive = false;
    private int _count = 0;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _light.color = _colorOff;
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
        _light.color = _isActive ? _colorOn : _colorOff;
        //_animator.SetBool("isActive", _isActive);
        PlaySound(_sfxOn, _sfxOff);
        ActivateLamp(_isActive);
        _event?.Invoke();

        if(_interactMode == InteractMode.Toggle)
        {
            StartCoroutine(ResetRoutine()); 
        }
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
        _light.color = _colorOff;
        _animator.SetBool("isActive", false);
        PlaySound(_sfxOn, _sfxOff);
        ActivateLamp(_isActive);
    }

    private void ActivateLamp(bool isActive)
    {
        _meshRenderer.material.SetColor("_EmissionColor", isActive ? _colorOn : _colorOff);   
    }

    IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(_resetDelay);
        ResetSwitch();
    }
}