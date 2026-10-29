using UnityEngine;

public class DronShooter : DronBase
{
    [Header("Shooter")]
    [SerializeField] Weapon _weapon;
    [SerializeField] float _shootRange = 12f;
    [SerializeField] float _shootRangeHysteresis = 2f;

    bool _inRange;

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);

        if (_weapon != null)
            _weapon.SetTarget(_target);
    }

    protected override bool ShouldHoldPosition() => _state == DroneStates.Chase && _inRange;

    protected override void Tick()
    {
        bool engaged = _state == DroneStates.Chase && _seesTarget && _target != null;

        _weapon.SetTracking(engaged);

        if (!engaged)
        {
            _inRange = false;
            return;
        }

        float dist = Vector3.Distance(transform.position, _target.position);

        _inRange = _inRange
            ? dist < _shootRange + _shootRangeHysteresis
            : dist < _shootRange;

        if (_inRange)
            _weapon.TryShoot();
    }
}