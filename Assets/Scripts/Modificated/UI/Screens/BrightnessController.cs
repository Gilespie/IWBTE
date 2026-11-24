using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] Volume _volume; 
    public const float MinExposure = -0.8f;
    public const float Step = 0.2f;
    public const int StepsCount = 9;

    static event Action<int> OnBrightnessChanged;
    ColorAdjustments _colorAdjustments;

    public static float IndexToExposure(int index)
    {
        index = Mathf.Clamp(index, 0, StepsCount - 1);
        return Mathf.Round((MinExposure + index * Step) * 10f) / 10f;
    }

    public static void Raise(int index) => OnBrightnessChanged?.Invoke(index);

    void OnEnable()
    {
        OnBrightnessChanged += Apply;
    }

    void Awake()
    {
        if (_volume == null) _volume = GetComponent<Volume>();

        if (!_volume.profile.TryGet(out _colorAdjustments))
            Debug.LogWarning("Hasn't Color Adjustments", this);
    }

    void Start()
    {
        if (SaveManager.Instance != null)
            Apply(SaveManager.Instance.Settings.BrightnessIndex);
    }

    void OnDisable()
    {
        OnBrightnessChanged -= Apply;
    }

    void Apply(int index)
    {
        if (_colorAdjustments == null) return;

        _colorAdjustments.postExposure.overrideState = true;
        _colorAdjustments.postExposure.value = IndexToExposure(index);
    }
}