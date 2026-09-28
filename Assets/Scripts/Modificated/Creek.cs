using UnityEngine;

public class Creek : MonoBehaviour
{
    [SerializeField] Vector3 _velocityFlow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable character))
        {
            character.InstantKill();
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            if(rb.gameObject.layer == LayerMask.NameToLayer("Ragdoll"))
            {
                rb.velocity = _velocityFlow;
            }
        }
    }
}
