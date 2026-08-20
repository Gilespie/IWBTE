using UnityEngine;
using UnityEngine.VFX;

public class Earthquake : MonoBehaviour
{
    [SerializeField] AudioClip[] _clips;
    [SerializeField] ParticleSystem[] _dusts;
    [SerializeField] float _timeToQuake = 15f;
    [SerializeField] VisualEffect[] _dustVFX;
    AudioSource _audioSource;
    float _currentTimer = 0f;
    int _randomClipIndex = 0;
    float _randomPitch = 0f;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /*void Start()
    {
        _currentTimer = _timeToQuake;
        //Shockwaves();
    }*/

   /* void Update()
    {
        _currentTimer -= Time.deltaTime;
        
        if(_currentTimer < 0f)
        {
            _currentTimer = _timeToQuake;
            Shockwaves();
        }
    }*/

    public void Shockwaves()
    {
        _randomClipIndex = Random.Range( 0, _clips.Length);
        _randomPitch = Random.Range(0.5f, 1f);
        //EventManager.Trigger(EventType.OnExplosion);

        if (_audioSource != null)
        {
            _audioSource.pitch = _randomPitch;
            _audioSource.PlayOneShot(_clips[_randomClipIndex]);
        }

        if (_dusts != null)
        {
            foreach (var dust in _dusts)
            {
                dust.Play();
            }
        }

        if (_dustVFX != null)
        {

            foreach (var dust in _dustVFX)
            {
                dust.SendEvent(EventType.OnExplosion.ToString());
            }
        }

        CameraShake.Instance.ActiveShake();
    }
}