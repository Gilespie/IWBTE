using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    private Animator _animator;

    private void Awake()
    {
        _panel.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.OnEndGame, ShowPanel);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnEndGame, ShowPanel);
    }

    private void ShowPanel(params object[] parameters)
    {
        _panel.SetActive(true);
    }
}   