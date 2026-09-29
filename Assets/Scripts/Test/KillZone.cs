using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] float damageAmount = 20f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable destruct))
        {
            destruct.InstantKill();
        }
    }
}