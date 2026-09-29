using UnityEngine;

public class StreetLightCrash : MonoBehaviour
{
    [SerializeField] Animator _animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Tesla>())
        {
            _animator.enabled = true;
        }
    }
}