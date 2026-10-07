using System.Collections;
using TMPro;
using UnityEngine;

public class IntroSequence : MonoBehaviour, ISaveable
{
    public static bool PlayedThisSession;

    [Header("References")]
    [SerializeField] CameraPointFollow _camera;
    [SerializeField] CharacterInputController _input;
    [SerializeField] Airplane _airplane;
    [SerializeField] Animator _celularAnimator;
    [SerializeField] TextMeshProUGUI _titleText;

    [Header("Intro camera")]
    [SerializeField] Transform _introCameraPoint;
    [SerializeField] Transform _introLookTarget;

    [Header("Timing")]
    [SerializeField] float _airplaneDelay = 0f;
    [SerializeField] float _arriveThreshold = 0.2f;
    [SerializeField] float _maxDuration = 15f;
    [SerializeField] float _delayAfterIntro = 0f;

    bool _introPlayed;
    bool _ready;
    bool _started;

    void Awake()
    {
        _introPlayed = PlayedThisSession;
        SaveManager.Instance.Register(this);
    }

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnStartGame, OnAnyKey);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnStartGame, OnAnyKey);
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Unregister(this);
    }

    IEnumerator Start()
    {
        yield return null;

        if (_introPlayed)
        {
            SetTitleAlpha(0f);
            yield break;
        }

        _input.DisableAllInput();
        _camera.SetIntroPoint(_introCameraPoint, _introLookTarget);
        _ready = true;
    }

    void OnAnyKey(params object[] args)
    {
        if (!_ready || _started || _introPlayed) return;
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        _started = true;
        StartCoroutine(FadeTitle());
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        StartCoroutine(PlayAirplane());

        yield return new WaitForSeconds(_delayAfterIntro);
        
        _celularAnimator.enabled = true;
        _camera.StartResetIntro();

        float t = 0f;

        while (!_camera.IsNearFollowPosition(_arriveThreshold) && t < _maxDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        FinishIntro();
    }

    IEnumerator PlayAirplane()
    {
        if (_airplaneDelay > 0f)
            yield return new WaitForSeconds(_airplaneDelay);

        _airplane.PlayAnim();
    }

    IEnumerator FadeTitle()
    {
        if (_titleText == null) yield break;

        Color c = _titleText.color;

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime;
            _titleText.color = c;
            yield return null;
        }
    }

    void SetTitleAlpha(float a)
    {
        if (_titleText == null) return;

        Color c = _titleText.color;
        c.a = a;
        _titleText.color = c;
    }

    void FinishIntro()
    {
        _introPlayed = true;
        PlayedThisSession = true;
        _input.EnableMovementMap();
    }

    public void CaptureState(SaveGameData data)
    {
        data.IntroPlayed = _introPlayed;
    }

    public void RestoreState(SaveGameData data)
    {
        _introPlayed |= data.IntroPlayed;
    }
}