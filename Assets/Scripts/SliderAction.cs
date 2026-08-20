using UnityEngine;
using UnityEngine.UI;

public class SliderAction : MonoBehaviour
{
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _voiceSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _uiSlider;

    void Start()
    {
        _masterSlider.value = AudioManager.Instance.InitMasterVol;
        _musicSlider.value = AudioManager.Instance.InitMusicVol;
        _voiceSlider.value = AudioManager.Instance.InitVoiceVol;
        _sfxSlider.value = AudioManager.Instance.InitSFXVol;
        _uiSlider.value = AudioManager.Instance.InitUIVol;
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void SetUIVolume(float value)
    {
        AudioManager.Instance.SetUIVolume(value);
    }

    public void SetVoiceVolume(float value)
    {
        AudioManager.Instance.SetVoiceVolume(value);
    }
}
