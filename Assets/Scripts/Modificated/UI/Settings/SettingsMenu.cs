using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Selectors")]
    [SerializeField] private OptionSelector _resolutionSelector;
    [SerializeField] private OptionSelector _qualitySelector;
    [SerializeField] private OptionSelector _vSyncSelector;
    [SerializeField] private OptionSelector _fullscreenSelector;
    [SerializeField] private List<Resolution> _resolutions = new();

    void Awake()
    {
        InitResolutionSelector();
        InitQualitySelector();
        InitVSyncSelector();
        InitFullscreenSelector();

        _resolutionSelector.OnValueChanged += ChangeResolution;
        _qualitySelector.OnValueChanged += ChangeQuality;
        _vSyncSelector.OnValueChanged += ChangeVSync;
        _fullscreenSelector.OnValueChanged += ChangeFullscreen;

    }

    private void OnDestroy()
    {
        _resolutionSelector.OnValueChanged -= ChangeResolution;
        _qualitySelector.OnValueChanged -= ChangeQuality;
        _vSyncSelector.OnValueChanged -= ChangeVSync;
        _fullscreenSelector.OnValueChanged -= ChangeFullscreen;
    }

    void InitResolutionSelector()
    {
        List<string> values = new();

        foreach (Resolution res in Screen.resolutions)
        {
            string text = $"{res.width} x {res.height}";

            if (values.Contains(text))
                continue;

            values.Add(text);
            _resolutions.Add(res);
        }

        int currentIndex = 0;

        for (int i = 0; i < _resolutions.Count; i++)
        {
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
                break;
            }
        }

        _resolutionSelector.Initialize(values, currentIndex);
    }

    void InitQualitySelector()
    {
        List<string> quality = new(QualitySettings.names);
        _qualitySelector.Initialize(quality, SaveManager.Instance.Settings.QualityIndex);
    }

    void InitVSyncSelector()
    {
        _vSyncSelector.Initialize(new List<string>{"OFF", "ON"}, SaveManager.Instance.Settings.VSync);
    }

    void InitFullscreenSelector()
    {
        _fullscreenSelector.Initialize(new List<string>{"OFF", "ON"}, SaveManager.Instance.Settings.Fullscreen ? 1 : 0);
    }

    void InitVolume()
    {

    }

    void ChangeResolution(int index)
    {
        SaveManager.Instance.Settings.ResolutionIndex = index;

        Resolution res = _resolutions[index];

        Screen.SetResolution(
            res.width,
            res.height,
            SaveManager.Instance.Settings.Fullscreen);

        SaveManager.Instance.SaveSettings();
    }

    void ChangeQuality(int index)
    {
        SaveManager.Instance.Settings.QualityIndex = index;
        SaveManager.Instance.Apply();
        SaveManager.Instance.SaveSettings();
    }

    void ChangeFullscreen(int index)
    {
        SaveManager.Instance.Settings.Fullscreen = index == 1;

        SaveManager.Instance.Apply();
        SaveManager.Instance.SaveSettings();
    }

    void ChangeVSync(int index)
    {
        SaveManager.Instance.Settings.VSync = index;

        SaveManager.Instance.Apply();
        SaveManager.Instance.SaveSettings();
    }

    void ChangeVolume(int value)
    {

    }
}