using UnityEngine;
using UnityEngine.UI;

public class CheckpointUI : MonoBehaviour
{
    [SerializeField] private Image _saveIcon;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private Animation _animation;

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.OnCheckpoint, OnCheckpoint);
    }

    private void Start()
    {
        _saveIcon.color = new(_saveIcon.color.r, _saveIcon.color.g, _saveIcon.color.b, 0);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnCheckpoint, OnCheckpoint);
    }

    private void OnCheckpoint(params object[] parameters)
    {
        _animation.Stop();
        _animation.Play();
        //AudioManager.Instance.PlayUI(_clip);
    }
}