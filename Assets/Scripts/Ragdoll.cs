using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _ragdollRBs;
    [SerializeField] private Animator _animator;

    void Start()
    {
        DisableRagdoll();
        DeactivateCollsion();
    }

    public void DisableRagdoll()
    {
        foreach (var rb in _ragdollRBs)
        {
            rb.isKinematic = true;
        }

        _animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        _animator.enabled = false;

        foreach (var rb in _ragdollRBs)
        {
            rb.isKinematic = false;
        }

    }

    public void DeactivateCollsion()
    {
        Physics.IgnoreLayerCollision(8, 11, true);
        
    }

    public void ActivateCollision()
    {
        Physics.IgnoreLayerCollision(8, 11, false);
    }
}