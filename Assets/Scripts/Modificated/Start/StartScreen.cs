using System.Collections;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [SerializeField] Animator _playerAnimator;
    [SerializeField] TextMeshProUGUI _titleText;
    Coroutine _fadeCoroutine;
    bool _started;

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnStartGame, StartGame);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnStartGame, StartGame);
    }

    void StartGame(params object[] args)
    {
        if (_started)
            return;

        _started = true;

        if(_fadeCoroutine == null)
            _fadeCoroutine = StartCoroutine(FadeText());

        //gameObject.SetActive(false);
        
        _playerAnimator.SetTrigger("StandUp");
    }

    IEnumerator FadeText()
    {
        Color colorA = _titleText.color;

        while(colorA.a > 0)
        {
            colorA.a -= Time.deltaTime;
            _titleText.color = colorA;
            yield return null;
        }

        _fadeCoroutine = null;
    }
}