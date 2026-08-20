using System.Collections;
using UnityEngine;

public class RadialRigidbodiesActivation : MonoBehaviour
{
    [SerializeField] private LayerMask m_ActivationLayer;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_Speed;

    private void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, m_Range, m_ActivationLayer.value);

        foreach (Collider c in colliders)
        {
            var rb = c.gameObject.GetComponent<Rigidbody>();
            var distance = Vector3.Distance(transform.position, c.gameObject.transform.position);
            StartCoroutine(ActivateRigidbodies(rb, distance / m_Speed));
        }
    }

    IEnumerator ActivateRigidbodies(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_Range);
    }
}