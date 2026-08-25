using UnityEngine;

public class GroundRaycast : AbstractRaycast
{
    public event System.Action<MaterialType> OnSurfaceHit;

    [Header("Slope")]
    [SerializeField] private float _maxSlopeAngle = 26.5f;
    [SerializeField] private LayerMask _slopeLayerMask;
    private float _currentSlopeAngle;
    private Vector3 _normalOrient;

    public Vector3 Normal => _normalOrient;
    public float SlopeAngle => _currentSlopeAngle;
    public bool IsSlopeTooSteep { get; private set; }

    [Header("Footstep")]
    private IStepable _stepable;
    private MaterialType _currentMaterial;

    private void Update()
    {
        bool hit = IsRaycasting(Vector3.down);

        if (hit)
        {
            _normalOrient = _hit.normal;
            _currentSlopeAngle = Vector3.Angle(_hit.normal, Vector3.up);

            bool isSlopeLayer = IsInLayerMask(_hit.collider.gameObject.layer, _slopeLayerMask);
            bool isAngleExtreme = _currentSlopeAngle >= _maxSlopeAngle;

            IsSlopeTooSteep = isSlopeLayer && isAngleExtreme;

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
            IsSlopeTooSteep = false;

            if (_currentMaterial != MaterialType.None)
            {
                _currentMaterial = MaterialType.None;
                OnSurfaceHit?.Invoke(_currentMaterial);
            }
        }
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}