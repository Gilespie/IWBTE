using UnityEngine;

public class SlopeMovement : MovementAdvance
{
    [SerializeField] GroundRaycast _groundRaycast;
    [SerializeField] float _maxSlideSpeed = 6f;
    [SerializeField] private float _slideAcceleration = 4f;

    private float _currentSlideSpeed;

    public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        Vector3 direction = transform.forward * dir.z;

        _rb.MovePosition(_rb.position + direction.normalized * _speedMovement * Time.fixedDeltaTime);
    }
}