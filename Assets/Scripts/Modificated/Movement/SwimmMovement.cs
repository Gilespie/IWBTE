using UnityEngine;

public class SwimmMovement : MovementAdvance
{
    /*public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        _direction = dir;

        float targetSpeed = _direction.sqrMagnitude > 0.01f ? _speedMovement : 0f;

        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        *//*Vector3 velocity = _direction.normalized * _currentSpeed;

        _rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);*//*

        _rb.linearVelocity = _direction.normalized * _currentSpeed;
    }*/

    public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        UpdateSmoothedDirection(dir);
        UpdateSpeed(dir.sqrMagnitude > 0.01f);

        Vector3 velocity = _smoothedDirection.sqrMagnitude > 0.0001f
            ? _smoothedDirection.normalized * _currentSpeed
            : Vector3.zero;

        _rb.linearVelocity = velocity;
    }
}