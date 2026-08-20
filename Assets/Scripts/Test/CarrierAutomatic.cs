using UnityEngine;

public class CarrierAutomatic : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void ActivateCarrier()
    {
        _animator.SetTrigger("OnActive");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.transform.SetParent(null);
        }
    }
}