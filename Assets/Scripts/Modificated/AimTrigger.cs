using UnityEngine;

public class AimTrigger : MonoBehaviour
{
    [SerializeField] private float _radius;

    private void OnTriggerEnter(Collider other)
    {
        AimController aim = other.GetComponent<AimController>();
        
        if (aim != null) aim.SetTarget(transform);
    }

    private void OnTriggerStay(Collider other)
    {
        AimController aim = other.GetComponent<AimController>();

        if (aim != null) aim.SetAimPosition(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<AimController>() != null)
        {
            AimController aim = other.GetComponent<AimController>();

            if (aim != null) aim.SetTarget(null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}