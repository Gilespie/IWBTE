using UnityEngine;

public class Creek : MonoBehaviour
{
    [SerializeField] Vector3 _velocityFlow;
    [SerializeField] float _flowStrength = 5f;
    [SerializeField] LayerMask _ragdollMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable character))
        {
            character.InstantKill();
        }   
    }

    private void OnTriggerStay(Collider other)
    {
        if ((_ragdollMask.value & (1 << other.gameObject.layer)) == 0) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        Vector3 current = rb.linearVelocity;

        Vector3 target = new Vector3(_velocityFlow.x, _velocityFlow.y, _velocityFlow.z);

        rb.AddForce((target - current) * _flowStrength, ForceMode.Acceleration);
    }
}