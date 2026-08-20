using System.Collections;
using UnityEngine;

public class PhysicObject : MonoBehaviour
{
    [SerializeField] float _intensity = 0.01f;
    [SerializeField] float _delay = 0.5f;
    [SerializeField] AudioClip[] _clips;
    Rigidbody _rb;
    AudioSource _audioSource;
    int _randomIndex;
    int _count;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _randomIndex = Random.Range(0, _clips.Length);
    }

    public void ToggleKinematic(bool value)
    {
        _rb.isKinematic = value;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_count > 1) return;

        _audioSource.volume = collision.impulse.magnitude * _intensity;
        _audioSource.PlayOneShot(_clips[_randomIndex]);

        if (collision.collider != null)
            ActivateRoutine();

        _count++;
    }

    void ActivateRoutine()
    {
        StartCoroutine(KinematicRoutine());
    }

    IEnumerator KinematicRoutine()
    {
        yield return new WaitForSeconds(_delay);
        ToggleKinematic(true);
        yield return null;
    }
}