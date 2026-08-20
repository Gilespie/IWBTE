using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float _delayToActivate;
    [SerializeField] private Light[] _lights;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        RocketExplosion.OnExplosed += OnTurnOff;
    }

    private void OnDisable()
    {
        RocketExplosion.OnExplosed -= OnTurnOff;
    }

    public void OnTurnOff()
    {
        Invoke(nameof(TurnOff), _delayToActivate);
    }

    void TurnOff()
    {
        _audioSource.Play();

        foreach (Light light in _lights)
        {
            light.gameObject.SetActive(false);
        }
    }
}