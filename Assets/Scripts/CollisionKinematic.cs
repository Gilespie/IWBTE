using UnityEngine;

public class CollisionKinematic : MonoBehaviour
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Airplane>())
        {
            _rb.isKinematic = false;
        }
    }
}