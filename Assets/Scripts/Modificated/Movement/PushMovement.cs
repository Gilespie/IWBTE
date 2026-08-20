using UnityEngine;

public class PushMovement : MovementAdvance
{
    /*public override void Advance(Vector3 dir,Vector3 extVelocity)
    {
        PushableBox box = _rb.GetComponentInParent<Character>().CurrentBox;

        if (box == null) return;

        _direction = new Vector3(dir.x, 0, dir.z);

        float targetSpeed = _direction.sqrMagnitude > 0.01f ? _speedMovement : 0f;

        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        *//* Vector3 velocity = _direction.normalized * _currentSpeed;

         _rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);

         Rigidbody boxRb = box.GetComponent<Rigidbody>();
         boxRb.MovePosition(boxRb.position + velocity * Time.fixedDeltaTime);*//*
        
        Vector3 horizontal = (_direction.normalized * _currentSpeed) + extVelocity;
        float verticalVelocity = _rb.linearVelocity.y;

        _rb.linearVelocity = new Vector3(horizontal.x, verticalVelocity, horizontal.z);

        Rigidbody boxRb = box.GetComponent<Rigidbody>();

        if (boxRb != null)
        {
            boxRb.MovePosition(boxRb.position + new Vector3(horizontal.x, 0f, horizontal.z) * Time.fixedDeltaTime);
        }
    }*/

    public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        PushableBox box = _rb.GetComponentInParent<Character>().CurrentBox;

        if (box == null) return;

        Vector3 targetDirection = new Vector3(dir.x, 0, dir.z);

        UpdateSmoothedDirection(targetDirection);
        UpdateSpeed(targetDirection.sqrMagnitude > 0.01f);

        Vector3 horizontal = BuildHorizontalVelocity(extVelocity);

        float verticalVelocity = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector3(horizontal.x, verticalVelocity, horizontal.z);

        Rigidbody boxRb = box.GetComponent<Rigidbody>();

        if (boxRb != null)
        {
            boxRb.MovePosition(boxRb.position + new Vector3(horizontal.x, 0f, horizontal.z) * Time.fixedDeltaTime);
        }
    }
}