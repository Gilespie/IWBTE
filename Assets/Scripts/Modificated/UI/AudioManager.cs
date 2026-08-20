using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    #region Instance
    public static AudioManager Instance;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }
    #endregion

    [Header("Variables")]
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] float _initMasterVol = 0.75f;
    [SerializeField] float _initMusicVol = 0.5f;
    [SerializeField] float _initVoiceVol = 0.5f;
    [SerializeField] float _initSFXVol = 0.5f;
    [SerializeField] float _initUIVol = 0.2f;
    [SerializeField] string _masterFloatName = "MasterVolume";
    [SerializeField] string _musicFloatName = "MusicVolume";
    [SerializeField] string _voiceFloatName = "VoiceVolume";
    [SerializeField] string _sfxFloatName = "SFXVolume";
    [SerializeField] string _uiFloatName = "UIVolume";

    public float InitMasterVol => _initMasterVol;
    public float InitMusicVol => _initMusicVol;
    public float InitVoiceVol => _initVoiceVol;
    public float InitSFXVol => _initSFXVol;
    public float InitUIVol => _initUIVol;

    private AudioSource _audioSource;

    void Start()
    {
        SetMasterVolume(InitMasterVol);
        SetMusicVolume(InitMusicVol);
        SetVoiceVolume(InitVoiceVol);
        SetSFXVolume(InitSFXVol);
        SetUIVolume(InitUIVol);
    }

    public void SetMasterVolume(float value)
    {
        if (value <= 0f) value = 0.0001f;

        _audioMixer.SetFloat(_masterFloatName, Mathf.Log(value) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        if (value <= 0f) value = 0.0001f;

        _audioMixer.SetFloat(_musicFloatName, Mathf.Log(value) * 20f);
    }

    public void SetVoiceVolume(float value)
    {
        if (value <= 0f) value = 0.0001f;

        _audioMixer.SetFloat(_voiceFloatName, Mathf.Log(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        if (value <= 0f) value = 0.0001f;

        _audioMixer.SetFloat(_sfxFloatName, Mathf.Log(value) * 20f);
    }

    public void SetUIVolume(float value)
    {
        if (value <= 0f) value = 0.0001f;

        _audioMixer.SetFloat(_uiFloatName, Mathf.Log(value) * 20f);
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