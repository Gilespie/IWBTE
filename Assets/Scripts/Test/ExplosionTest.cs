using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionTest : MonoBehaviour
{
    [SerializeField] VisualEffect[] effects;
    [SerializeField] float _delay = 10f;
    [SerializeField] AudioClip[] _clips;
    [SerializeField] AudioSource _audioSource;
    int _currentIndex = 0;

    private void OnEnable()
    {
        PlayEffects();
        StartCoroutine(ResetRoutine());
    }

    private void OnDisable()
    {
        StopEffects();
    }

    [ContextMenu("Play")]
    public void PlayEffects()
    {
        _currentIndex = Random.Range(0, _clips.Length);
        _audioSource.PlayOneShot(_clips[_currentIndex]);

        foreach (var effect in effects)
        {
            effect.Play();
        }
    }

    [ContextMenu("Stop")]
    private void StopEffects()
    {
        foreach (var effect in effects)
        {
            effect.Stop();
        }
    }

    IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(_delay);
        gameObject.SetActive(false);
        yield return null;
    }
}