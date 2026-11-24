using System.Collections;
using UnityEngine;

public class CameraShake : SingletonBase<CameraShake>
{
    /* [Header("Shake Intensity")]
     [SerializeField, Range(1f, 10f)] float _force = 5f;
     [SerializeField] float _shakeDuration = 1.5f;
     [SerializeField] float _speedTransition = 0.1f;
     Vector3 _shakeOffset;
     [SerializeField]CameraFollow _cameraPoint;

     void Start()
     {
         _cameraPoint = GameManager.Instance.CameraPointFollow;
     }

     public void ActiveShake()
     {
         StartCoroutine(ShakeCamera(_force, _shakeDuration));
     }

     IEnumerator ShakeCamera(float force, float duration)
     {
         _shakeOffset = Vector3.zero;
         float elapsed = 0.0f;

         while (elapsed < duration)
         {
             Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * force;

             _shakeOffset = Vector3.Lerp(_shakeOffset, offset, _speedTransition);

             _cameraPoint.SetShakeOffset(_shakeOffset);

             elapsed += Time.fixedDeltaTime;

             yield return null;
         }

         _cameraPoint.SetShakeOffset(Vector3.zero);
     }*/

    /*IEnumerator ShakeCamera(float force, float duration)
    {
        float elapsed = 0f;

        // Случайный seed, чтобы каждая тряска была уникальной
        float seedX = Random.Range(0f, 100f);
        float seedY = Random.Range(0f, 100f);
        float noiseSpeed = 2f; // скорость "прокрутки" шума

        while (elapsed < duration)
        {
            // t от 1 до 0 — затухание к концу
            float t = 1f - (elapsed / duration);
            float currentForce = force * t;

            // Perlin Noise возвращает значения 0..1, сдвигаем в -1..1
            float x = (Mathf.PerlinNoise(seedX + elapsed * noiseSpeed, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, seedY + elapsed * noiseSpeed) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y, 0f) * currentForce;
            _cameraPoint.SetShakeOffset(offset);

            elapsed += Time.deltaTime; // deltaTime вместо fixedDeltaTime
            yield return null;
        }

        // Плавное возвращение в ноль
        yield return StartCoroutine(SmoothReset());
    }

    IEnumerator SmoothReset()
    {
        Vector3 lastOffset = *//* сохранить последний offset *//* Vector3.zero;
        float resetDuration = 0.15f;
        float t = 0f;

        while (t < resetDuration)
        {
            t += Time.deltaTime;
            Vector3 offset = Vector3.Lerp(lastOffset, Vector3.zero, t / resetDuration);
            _cameraPoint.SetShakeOffset(offset);
            yield return null;
        }

        _cameraPoint.SetShakeOffset(Vector3.zero);
    }*/

    [Header("Shake Intensity")]
    [SerializeField] float _force = 0.3f;          // амплитуда в мировых единицах
    [SerializeField] float _shakeDuration = 1.5f;
    [SerializeField] float _frequency = 6f;        // чем меньше, тем плавнее (3-8)

    [Header("Envelope")]
    [SerializeField] float _fadeInTime = 0.15f;    // плавное нарастание
    [SerializeField] AnimationCurve _fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Axes")]
    [SerializeField] float _verticalMultiplier = 0.6f; // вертикаль слабее - меньше укачивает
    [SerializeField] CameraFollow _cameraPoint;

    Coroutine _routine;

    void Start()
    {
        _cameraPoint = GameManager.Instance.CameraPointFollow;
    }

    public void ActiveShake()
    {
        ActiveShake(_force, _shakeDuration);
    }

    public void ActiveShake(float force, float duration)
    {
        if (_cameraPoint == null) return;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(ShakeCamera(force, duration));
    }

    IEnumerator ShakeCamera(float force, float duration)
    {
        float elapsed = 0f;
        float seedX = Random.Range(0f, 100f);
        float seedY = Random.Range(100f, 200f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float fadeIn = _fadeInTime > 0f ? Mathf.Clamp01(elapsed / _fadeInTime) : 1f;
            float fadeOut = _fadeOutCurve.Evaluate(t);
            float envelope = fadeIn * fadeOut;

            float x = (Mathf.PerlinNoise(seedX, elapsed * _frequency) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(seedY, elapsed * _frequency) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y * _verticalMultiplier, 0f) * (force * envelope);
            _cameraPoint.SetShakeOffset(offset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _cameraPoint.SetShakeOffset(Vector3.zero);
        _routine = null;
    }
}