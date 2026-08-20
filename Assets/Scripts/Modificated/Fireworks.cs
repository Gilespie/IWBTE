using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Fireworks : MonoBehaviour
{
    [SerializeField] VisualEffect _fireworksEffect;
    [SerializeField] float _speedFade = 15f;
    //[SerializeField] Light _light;
    [SerializeField] private float _emmisiveIntensity = 8f;

    private void OnEnable()
    {
        _fireworksEffect.outputEventReceived += OnVFXEvent;
    }

    private void OnDisable()
    {
        _fireworksEffect.outputEventReceived -= OnVFXEvent;
    }

    private void OnVFXEvent(VFXOutputEventArgs args)
    {
        if (args.nameId == Shader.PropertyToID("OnDead"))
        {
            //ChangeLight();
            SetColor();
        }
    }

    private void SetColor()
    {
        Color newcolor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f) * _emmisiveIntensity;
        _fireworksEffect.SetVector4("MainColor", newcolor);
    }

   /* private void ChangeLight()
    {
        _light.color = _fireworksEffect.GetVector4("MainColor");
        _light.intensity = 1f;
        
        StartCoroutine(ColorRoutine());
    }

    IEnumerator ColorRoutine()
    {
        float t = 0;
        float startIntensity = _light.intensity;

        while (t < 1f)
        {
            t += Time.deltaTime * _speedFade;
            _light.intensity = Mathf.Lerp(startIntensity, 0, t);
            yield return null;
        }
    }*/
}