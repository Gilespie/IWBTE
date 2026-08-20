using System.Collections;
using UnityEngine;

public class FallBridge : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Collider _collider;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem[] _dustParticles;
    [SerializeField] private float _delay = 2f;
    private string _layerMask = "Slope";

    public void DestroyBridge()
    {
        StartCoroutine(StopHingeRoutine());

        _audioSource.Play();

        foreach (var dust in _dustParticles)
        {
            dust.gameObject.SetActive(true);
            dust.Play();
        }

        gameObject.layer = LayerMask.NameToLayer(_layerMask);
    }

    private IEnumerator StopHingeRoutine()
    {
        _rb.isKinematic = false;
        yield return new WaitForSeconds(_delay);
        _rb.isKinematic = true;
        yield return null;
    }
}