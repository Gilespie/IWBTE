using UnityEngine;
using UnityEngine.UI;

public class SliderAction : MonoBehaviour
{
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _voiceSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _uiSlider;

    void Start() => RefreshSliders();

    void OnEnable() => RefreshSliders();

    void OnDisable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveSettings();
    }

    void RefreshSliders()
    {
        if (AudioManager.Instance == null) return;

        _masterSlider.SetValueWithoutNotify(AudioManager.Instance.MasterVol);
        _musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVol);
        _voiceSlider.SetValueWithoutNotify(AudioManager.Instance.VoiceVol);
        _sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVol);
        _uiSlider.SetValueWithoutNotify(AudioManager.Instance.UIVol);
    }

    public void SetMasterVolume(float value) => AudioManager.Instance.SetMasterVolume(value);
    public void SetMusicVolume(float value) => AudioManager.Instance.SetMusicVolume(value);
    public void SetSFXVolume(float value) => AudioManager.Instance.SetSFXVolume(value);
    public void SetUIVolume(float value) => AudioManager.Instance.SetUIVolume(value);
    public void SetVoiceVolume(float value) => AudioManager.Instance.SetVoiceVolume(value);
}
