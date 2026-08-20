using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField] private Ray _groundRay;
    [SerializeField] private Transform _groundOrigin;
    private RaycastHit _groundHit;

    [Header("Settings")]
    [SerializeField] private float _groundRayDistance = 0.25f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded = false;

    private void Update()
    {
        IsGrounded();
    }

    public void IsGrounded()
    {
        _groundRay = new Ray(_groundOrigin.position, -transform.up);

        _isGrounded = Physics.Raycast(_groundRay, out _groundHit, _groundRayDistance,  _groundLayer);

        if(_isGrounded)
        {
            transform.up = _groundHit.collider.transform.up;
        }
        else
        {
            transform.up = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(_groundRay.origin, _groundRay.origin + _groundRay.direction * _groundRayDistance);
    }
}
