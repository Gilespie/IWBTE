using System.Collections;
using UnityEngine;

public class Lights : MonoBehaviour
{
    [SerializeField] private float _delayOn = 1.0f;
    [SerializeField] private float _delayOff = 1.0f;
    [SerializeField] private bool _isActivated = false;
    private Light _light;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Start()
    {
        StartCoroutine(BlinkLight(true));
    }

    private IEnumerator BlinkLight(bool activate)
    {
        while (activate)
        {
            yield return new WaitForSeconds(_delayOff);
            _light.enabled = true;
            yield return new WaitForSeconds(_delayOn);
            _light.enabled = false;

            yield return null;
        }


        yield return null;
    }

    public void TurnOffLight()
    {
        StopAllCoroutines();
        _light.enabled = false;
    }
}
