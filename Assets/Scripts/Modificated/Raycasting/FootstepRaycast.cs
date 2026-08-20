using UnityEngine;

public class FootstepRaycast : MonoBehaviour
{
    public event System.Action<MaterialType> OnSurfaceHit;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerMask;
    Ray _ray;
    RaycastHit _hit;
    bool _isHit = false;
    IStepable _stepable;
    MaterialType _currentMaterial;

    private void Update()
    {
        _ray = new Ray(_originPoint.position, Vector3.down);

        if (Physics.Raycast(_ray, out _hit, _rayDistance, _layerMask))
        {
            _isHit = true;
            _stepable = _hit.collider.GetComponent<IStepable>();

            MaterialType newMaterial = _stepable != null ? _stepable.MaterialType : MaterialType.None;

            if (newMaterial != _currentMaterial)
            {
                _currentMaterial = newMaterial;
                OnSurfaceHit?.Invoke(_currentMaterial);
            }
        }
        else
        {
            _isHit = false;

            if (_currentMaterial != MaterialType.None)
            {
                _currentMaterial = MaterialType.None;
                OnSurfaceHit?.Invoke(_currentMaterial);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isHit ? Color.green : Color.red;
        Gizmos.DrawRay(_originPoint.position, Vector3.down * _rayDistance);
    }
}