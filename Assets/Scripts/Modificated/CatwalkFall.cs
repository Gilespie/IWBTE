using UnityEngine;

public class CatwalkFall : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _delayToReset = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DronMovement drone))
        {
            _rb.isKinematic = false;
            Invoke(nameof(ResetKinematic), _delayToReset);
        }
    }

    void ResetKinematic()
    {
        _rb.isKinematic = true;
    }
}