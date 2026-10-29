using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Exploder))]
public class DroneKamikadze : DronBase
{
    [Header("Kamikaze")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] bool _isCutsceneDrone = false;
    [SerializeField] float _explodeDistance = 3f;

    [Header("Cutscene (Rigidbody only)")]
    [SerializeField] Transform _cutscenePoint;
    [SerializeField] float _cutsceneSpeed = 5f;

    Exploder _exploder;

    protected override void Awake()
    {
        base.Awake();
        _exploder = GetComponent<Exploder>();

        if (_isCutsceneDrone)
        {
            if (_agent != null) _agent.enabled = false;
            _rb.isKinematic = true;
        }
    }

    protected override void Start()
    {
        if (_isCutsceneDrone)
        {
            _state = DroneStates.Chase;
            SetLightColor(_chaseColor);
            return;
        }

        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (_isCutsceneDrone)
        {
            CutsceneTick();
            return;
        }

        base.FixedUpdate();
    }

    protected override void Tick()
    {
        if (_exploder.IsExploded || !_seesTarget) return;

        if ((_target.position - transform.position).sqrMagnitude <= _explodeDistance * _explodeDistance)
            _exploder.Explode();
    }

    void CutsceneTick()
    {
        if (_cutscenePoint == null || _exploder.IsExploded) return;

        Vector3 dir = _cutscenePoint.position - transform.position;

        if (dir.sqrMagnitude < _arrivalDistance * _arrivalDistance)
        {
            _exploder.Explode();
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            _rotationSpeed * Time.fixedDeltaTime);

        _rb.MovePosition(_rb.position + dir.normalized * _cutsceneSpeed * Time.fixedDeltaTime);

        HandleBeepSound(_cutscenePoint);
    }

    public void SetCutscenePoint(Transform point) => _cutscenePoint = point;
}