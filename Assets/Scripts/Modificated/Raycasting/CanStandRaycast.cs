using UnityEngine;

public class CanStandRaycast : MonoBehaviour        
{
    [SerializeField] float _distance = 1f;
    [SerializeField] float _radius = 0.5f;
    [SerializeField] LayerMask _obstacleMask;
    public bool CanStandUp()
    {
        return !Physics.CheckSphere(
        transform.position + Vector3.up * _distance,
        _radius,
        _obstacleMask,
        QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _distance);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _distance, _radius);
    }
}