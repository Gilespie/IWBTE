using UnityEngine;

public class InteractObject : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private bool _isActivated = false;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (_isActivated) return;    
        m_AudioSource.Play();
        _isActivated = true;
    }
}