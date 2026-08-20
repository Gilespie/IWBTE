using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_LightIntensity;
    private float currentTime;
    private Light _light;
    private float totaltime;

    void Start()
    {
        _light = GetComponent<Light>();

        totaltime = m_LightIntensity.keys[m_LightIntensity.length - 1].time;
    }

    void Update()
    {
        _light.intensity = m_LightIntensity.Evaluate(currentTime);

        currentTime += Time.deltaTime;

        if(currentTime > totaltime)
        {
            currentTime = 0f;
        }


    }
}
