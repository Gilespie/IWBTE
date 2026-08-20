using UnityEngine;

public class SubteDeadZoneView : MonoBehaviour
{
    [SerializeField] SubteDeadZone _deadZone;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _clip;

    private void Awake()
    {
        _deadZone.OnEntered += HandleEntered;
    }

    private void OnDestroy()
    {
        _deadZone.OnEntered -= HandleEntered;
    }

    private void HandleEntered()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.PlayOneShot(_clip);
        
        Debug.Log("¡El jugador ha entrado en la zona muerta del subte!");
    }
}
