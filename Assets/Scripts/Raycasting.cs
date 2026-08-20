using UnityEngine;

public class Raycasting : MonoBehaviour
{
    [Header("Rays")]
    private Ray _groundRay;
    private Ray _interactRay;
    private Ray _pushingRay;
    //private Ray _waterRay;
    private Ray[] _ceilingRay = new Ray[3];
    [SerializeField] private Transform _groundOrigin;
    [SerializeField] private Transform _interactOrigin;
    [SerializeField] private Transform _ceilingOrigin;
    private RaycastHit _groundHit;
    private RaycastHit _interactHit;
    private RaycastHit _pushingHit;
    private RaycastHit _ceilingHit;
   // private RaycastHit _waterHit;

    [Header("Settings")]
    [SerializeField] private float _groundRayDistance = 0.45f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _slopeLayer;
    [SerializeField] private float _interactRayDistance = 0.5f;
    [SerializeField] private LayerMask _interactLayer; 
    [SerializeField] private float _pushingRayDistance = 0.5f;
    [SerializeField] private LayerMask _pushingLayer;
    [SerializeField] private float _ceilingRayDistance = 0.3f;
/*    [SerializeField] private float _waterRayDistance = 0.1f;
    [SerializeField] private LayerMask _waterLayer;*/
    [SerializeField] private float _intRadius = 0.1f;
    [SerializeField] private float _maxSlopeAngle = 26.5f;
    private float _currentSlopeAngle = 0f;
    private Vector3 _normalOrient;
    public Vector3 Normal => _normalOrient;

    private float _yPos;
    public float YPos => _yPos;

    public bool IsGrounded()
    {
        _groundRay = new Ray(_groundOrigin.position, -transform.up);

        return Physics.Raycast(_groundRay, _groundRayDistance, _groundLayer);
    }

    public bool IsSlope()
    {
        _groundRay = new Ray(_groundOrigin.position, -transform.up);

        if (Physics.Raycast(_groundRay, out _groundHit, _groundRayDistance, _slopeLayer))
        {
            _currentSlopeAngle = Vector3.Angle(_groundHit.normal, Vector3.up);

            if (_currentSlopeAngle >= _maxSlopeAngle)
            {
                _normalOrient = _groundHit.normal;
                return true;
            }
        }

        return false;
    }

    /*public bool IsUnderWaterSurface()
    {
        Debug.Log("surface");
        return Physics.Raycast(
            _waterOrigin.position,
            Vector3.up,
            _waterRayDistance,
            _waterLayer,
            QueryTriggerInteraction.Collide
        );
    }*/

    public bool IsCeiling()
    {
/*      _ceilingRay = new Ray(_ceilingOrigin.position, transform.up);

        if(Physics.Raycast(_ceilingRay, out _ceilingHit, _ceilingRayDistance))
        {
            if(_ceilingHit.collider != null)
            {
                return true;
            }
        }

        return false;*/

        _ceilingRay[0] = new Ray(_ceilingOrigin.position, transform.up);
        _ceilingRay[1] = new Ray(_ceilingOrigin.position + transform.right * 0.3f, transform.up);
        _ceilingRay[2] = new Ray(_ceilingOrigin.position - transform.right * 0.3f, transform.up);

        for (int i = 0; i < _ceilingRay.Length; i++)
        {
            if (Physics.Raycast(_ceilingRay[i], out _ceilingHit, _ceilingRayDistance))
            {
                if (_ceilingHit.collider != null)
                {
                    return true; // хотя бы один луч попал в потолок
                }
            }
        }

        return false; // ни один луч не попал
    }

    public bool IsInteract()
    {
        _interactRay = new Ray(_interactOrigin.position, transform.forward);

        return Physics.Raycast(_interactRay, out _interactHit, _interactRayDistance, _interactLayer);
    }

    public bool IsPushing()
    {
        _pushingRay = new Ray(_interactOrigin.position, transform.forward);

        return Physics.Raycast(_pushingRay, out _pushingHit, _pushingRayDistance, _pushingLayer);
    }

    public void Interact()
    {
        _interactRay = new Ray(_interactOrigin.position, transform.forward);

        if (Physics.SphereCast(_interactRay, _intRadius, out _interactHit, _interactRayDistance, _interactLayer))
        {
            if (_interactHit.collider.TryGetComponent(out IInteractable interact))
            {
                interact.Interact();
            }
        }
    }

    public void Pushing()
    {
        _pushingRay = new Ray(_interactOrigin.position, transform.forward);

        if (Physics.SphereCast(_pushingRay, _intRadius, out _pushingHit, _pushingRayDistance, _pushingLayer))
        {
            if (_pushingHit.collider.TryGetComponent(out IPushable pushable))
            {
               // pushable.Pushing();
            }
        }
    }

    /* private void OnDrawGizmos()
     {
         bool isGrounded = false;
         bool isInteractable = false;
         bool isPushing = false;
         bool isCeiling = false;

         Gizmos.color = isGrounded ? Color.green : Color.red;
         Gizmos.DrawLine(_groundRay.origin, _groundRay.origin + _groundRay.direction * _groundRayDistance);

         _interactRay = new Ray(_interactOrigin.position, transform.forward);
         Gizmos.color = isInteractable ? Color.blue : Color.red;
         Gizmos.DrawLine(_interactRay.origin, _interactRay.origin + _interactRay.direction * _interactRayDistance);

         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(_interactHit.point, _intRadius);

         Gizmos.color = isPushing ? Color.green : Color.red;
         Gizmos.DrawLine(_pushingRay.origin, _pushingRay.origin + _pushingRay.direction * _pushingRayDistance);

         Gizmos.color = isCeiling ? Color.red : Color.green;
         Gizmos.DrawRay(_ceilingOrigin.position + transform.forward * 0.3f, Vector3.up * _ceilingRayDistance);
         Gizmos.DrawRay(_ceilingOrigin.position, Vector3.up * _ceilingRayDistance);
         Gizmos.DrawRay(_ceilingOrigin.position + -transform.forward * 0.3f, Vector3.up * _ceilingRayDistance);
     }*/
}