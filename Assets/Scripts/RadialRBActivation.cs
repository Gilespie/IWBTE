using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Install for player or object
/// </summary>
public class RadialRBActivation : MonoBehaviour
{
    [SerializeField] private bool _isUpdateEnable = true;
    [SerializeField] private LayerMask _activationLayer;
    [SerializeField] private float _range;
    [SerializeField] private float _speed;
    private Collider[] _colliders = new Collider[60];
    private List<Rigidbody> _ActivatedRB = new List<Rigidbody>();
    private int _hitCount = 0;

    private void OnEnable()
    {
        if (!_isUpdateEnable) ActivateRadialCrack();
    }

    private void Update()
    {
        if (_isUpdateEnable)
        {
            ActivateRadialCrack();
        }
    }

    private void ActivateRadialCrack()
    {
        _hitCount = Physics.OverlapSphereNonAlloc(transform.position, _range, _colliders ,_activationLayer);

        for (int i = 0; i < _hitCount; i++)
        {
            Rigidbody rb = _colliders[i].attachedRigidbody;

            if (rb != null && !_ActivatedRB.Contains(rb))
            {
                float distance = Vector3.Distance(transform.position, rb.position);
                StartCoroutine(ActivateRigidbodies(rb, distance / _speed));
                _ActivatedRB.Add(rb);
                rb.isKinematic = false;
            }
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
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}