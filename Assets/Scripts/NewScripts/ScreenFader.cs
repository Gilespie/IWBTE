using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] bool _startFadedIn = true;
    [SerializeField] float _fadeTime = 2f;
    [SerializeField] Image _image;
    Coroutine _currentRoutine;

    void Awake()
    {
        SetAlphaInstant(_startFadedIn ? 1f : 0f);
    }

    public Coroutine FadeIn() => StartFade(0f);
    
    public Coroutine FadeOut() => StartFade(1f);

    private Coroutine StartFade(float toAlpha)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }

        _currentRoutine = StartCoroutine(FadeRoutine(toAlpha));

        return _currentRoutine;
    }

    private IEnumerator FadeRoutine(float toAlpha)
    {
        float elapsed = 0f;
        Color color = _image.color;
        float fromAlpha = color.a;

        while (elapsed < _fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _fadeTime);
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            _image.color = color;
            yield return null;
        }

        color.a = toAlpha;
        _image.color = color;
        _currentRoutine = null;
    }

    private void SetAlphaInstant(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}