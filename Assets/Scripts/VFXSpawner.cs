using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem _prefabEffect;

    public void SpawnParticle(Transform target)
    {
        Instantiate(_prefabEffect.gameObject, target.position, Quaternion.identity);
        _prefabEffect.Play();
    }
}