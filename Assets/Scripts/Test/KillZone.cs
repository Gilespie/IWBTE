using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] float damageAmount = 20f;

    void OnTriggerEnter(Collider other)
    {
        Destructable destruct = other.gameObject.GetComponent<Destructable>();

        if (destruct != null)
        {
            destruct.InstantKill();
        }
    }
}