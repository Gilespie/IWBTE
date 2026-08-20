using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plank : MonoBehaviour
{
    Rigidbody _rb;

    private void Start()
    {
        _rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Character player))
        {
            _rb.isKinematic = false;
        }
    }
}