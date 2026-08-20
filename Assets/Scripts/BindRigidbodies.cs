using UnityEngine;

/// <summary>
/// Install for Sphere as orientation
/// </summary>
public class BindRigidbodies : MonoBehaviour
{
    [SerializeField] private LayerMask _layer;
    [SerializeField] private float _breakForce = Mathf.Infinity;
    [SerializeField] private float _breakTorque = Mathf.Infinity;
    private Collider[] _colliders;

    private void Awake()
    {
        _colliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, _layer.value);

        for (int i = 0; i < _colliders.Length - 1; i++)
        {
            FixedJoint joint = _colliders[i].gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = _colliders[i + 1].gameObject.GetComponent<Rigidbody>();
            joint.breakForce = _breakForce;
            joint.breakTorque = _breakTorque;
        }

        Destroy(gameObject);
    }
}