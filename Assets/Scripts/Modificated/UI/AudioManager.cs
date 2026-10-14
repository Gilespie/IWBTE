using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonBase<AudioManager>
{
    [Header("Variables")]
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] string _masterFloatName = "MasterVolume";
    [SerializeField] string _musicFloatName = "MusicVolume";
    [SerializeField] string _voiceFloatName = "VoiceVolume";
    [SerializeField] string _sfxFloatName = "SFXVolume";
    [SerializeField] string _uiFloatName = "UIVolume";

    public float MasterVol { get; private set; }
    public float MusicVol { get; private set; }
    public float VoiceVol { get; private set; }
    public float SFXVol { get; private set; }
    public float UIVol { get; private set; }

    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();

        var s = SaveManager.Instance.Settings;
        MasterVol = Mathf.Clamp01(s.MasterVolume);
        MusicVol = Mathf.Clamp01(s.MusicVolume);
        VoiceVol = Mathf.Clamp01(s.VoiceVolume);
        SFXVol = Mathf.Clamp01(s.SFXVolume);
        UIVol = Mathf.Clamp01(s.UIVolume);
    }

    void Start()
    {
        SetMasterVolume(MasterVol);
        SetMusicVolume(MusicVol);
        SetVoiceVolume(VoiceVol);
        SetSFXVolume(SFXVol);
        SetUIVolume(UIVol);
    }

    public void SetMasterVolume(float value)
    {
        MasterVol = Mathf.Clamp01(value);
        SaveManager.Instance.Settings.MasterVolume = MasterVol;
        ApplyToMixer(_masterFloatName, MasterVol);
    }

    public void SetMusicVolume(float value)
    {
        MusicVol = Mathf.Clamp01(value);
        SaveManager.Instance.Settings.MusicVolume = MusicVol;
        ApplyToMixer(_musicFloatName, MusicVol);
    }

    public void SetVoiceVolume(float value)
    {
        VoiceVol = Mathf.Clamp01(value);
        SaveManager.Instance.Settings.VoiceVolume = VoiceVol;
        ApplyToMixer(_voiceFloatName, VoiceVol);
    }

    public void SetSFXVolume(float value)
    {
        SFXVol = Mathf.Clamp01(value);
        SaveManager.Instance.Settings.SFXVolume = SFXVol;
        ApplyToMixer(_sfxFloatName, SFXVol);
    }

    public void SetUIVolume(float value)
    {
        UIVol = Mathf.Clamp01(value);
        SaveManager.Instance.Settings.UIVolume = UIVol;
        ApplyToMixer(_uiFloatName, UIVol);
    }

    void ApplyToMixer(string param, float value)
    {
        _audioMixer.SetFloat(param, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_audioSource.isPlaying) _audioSource.Stop();

        _audioSource.clip = clip;

        _audioSource.Pause();
    }

    public void PlayUI(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}