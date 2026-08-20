using UnityEngine;

namespace Shooter
{
    public class BindRigidbodies : MonoBehaviour
    {
        [SerializeField] private LayerMask m_Layer;
        [SerializeField] private float m_BreakForce = Mathf.Infinity;
        [SerializeField] private float m_BreakTorque = Mathf.Infinity;

        private void Awake()
        {
            var collider = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, m_Layer.value);

            for (int i = 0; i < collider.Length - 1; i++)
            {
                FixedJoint joint = collider[i].gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = collider[i + 1].gameObject.GetComponent<Rigidbody>();
                joint.breakForce = m_BreakForce;
                joint.breakTorque = m_BreakTorque;
            }

            Destroy(gameObject);
        }
    }
}