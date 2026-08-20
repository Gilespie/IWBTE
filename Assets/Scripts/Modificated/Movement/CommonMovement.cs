using UnityEngine;

public class CommonMovement : MovementAdvance
{
    public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        /*_direction = new Vector3(dir.x, 0, dir.z);

        if (_direction.sqrMagnitude > 0.01f)
        {
            _lastDirection = _direction.normalized;
        }

        float targetSpeed = _direction.sqrMagnitude > 0.01f ? _speedMovement : 0f;

        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        *//*Vector3 velocity = (_lastDirection * _currentSpeed) + _externalVelocity;

        _rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);*//*

        Vector3 horizontal = (_lastDirection * _currentSpeed) + extVelocity;

        float verticalVelocity = _rb.linearVelocity.y;

        _rb.linearVelocity = new Vector3(horizontal.x, verticalVelocity, horizontal.z);*/

        Vector3 targetDirection = new Vector3(dir.x, 0, dir.z);

        UpdateSmoothedDirection(targetDirection);
        UpdateSpeed(targetDirection.sqrMagnitude > 0.01f);

        Vector3 horizontal = BuildHorizontalVelocity(extVelocity);

        float verticalVelocity = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector3(horizontal.x, verticalVelocity, horizontal.z);
    }

    public override void Jump()
    {
        base.Jump();
    }
}