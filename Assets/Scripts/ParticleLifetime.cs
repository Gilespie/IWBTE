using UnityEngine;

public class ParticleLifetime : MonoBehaviour
{
    [SerializeField] private float _delayToDestroy = 4f;

    void Start()
    {
        Destroy(gameObject, _delayToDestroy);
    }
}