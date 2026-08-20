using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    [SerializeField] AudioSource _source;
    [SerializeField] float _delay = 1f;
    public void PlaySound()
    {
        Invoke(nameof(DelaySound), _delay);
    }

    void DelaySound()
    {
        _source.Play();
    }
}
