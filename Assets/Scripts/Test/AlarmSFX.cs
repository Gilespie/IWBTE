using System.Collections;
using UnityEngine;

public class AlarmSFX : MonoBehaviour
{
    [SerializeField] AudioSource[] _audios;

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnLiftFalled, ActivateRoutine);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnLiftFalled, ActivateRoutine);
    }

    public void ActivateRoutine(params object[] parameters)
    {
        StartCoroutine(AlarmCoroutine());
    }

    IEnumerator AlarmCoroutine()
    {
        _audios[0].Play();
        yield return new WaitForSeconds(1);
        _audios[1].Play();
        yield return new WaitForSeconds(1);
        _audios[2].Play();
        yield return null;
    }
}
