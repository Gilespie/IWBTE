using UnityEngine;

public class DeactivateZone : MonoBehaviour
{
    [SerializeField] LayerMask _mask;

    private void OnTriggerEnter(Collider other)
    {
        if ((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.gameObject.SetActive(false);
        }
    }
}