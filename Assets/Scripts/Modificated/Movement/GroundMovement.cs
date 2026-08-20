using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] protected float _crouchSpeed = 3f;
    [SerializeField] protected float _runSpeed = 5f;
    [SerializeField] protected float _sprintSpeed = 8f;
    protected float _currentSpeed;

    [SerializeField] protected float _inputSmoothSpeed = 8f;

    [Header("Acceleration")]
    [SerializeField] protected float _acceleration = 10f;
    [SerializeField] protected float _deceleration = 10f;

    [Header("Jump")]
    [SerializeField] protected float _jumpForce = 5f;

    protected Rigidbody _rb;
    protected CapsuleCollider _capsule;

    protected Vector3 _smoothedDirection;
    public Vector3 SmoothedDirection => _smoothedDirection;

    protected void UpdateSpeed(bool hasInput, float speed)
    {
        float targetSpeed = hasInput ? speed : 0f;
        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
    }

    public virtual void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    /*public void Advance(Vector3 dir)
    {
        Vector3 targetDirection = new Vector3(dir.x, 0, dir.z);

        UpdateSmoothedDirection(targetDirection);
        UpdateSpeed(targetDirection.sqrMagnitude > 0.01f, _runSpeed);

        Vector3 horizontal = BuildHorizontalVelocity();

        float verticalVelocity = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector3(horizontal.x, verticalVelocity, horizontal.z);
    }*/

    protected void UpdateSmoothedDirection(Vector3 rawDir)
    {
        if (rawDir.sqrMagnitude > 0.01f)
        {
            Vector3 targetNormalized = rawDir.normalized;

            Vector3 currentNormalized = _smoothedDirection.sqrMagnitude > 0.0001f
                ? _smoothedDirection.normalized
                : targetNormalized;

            _smoothedDirection = Vector3.Slerp(
                currentNormalized,
                targetNormalized,
                _inputSmoothSpeed * Time.fixedDeltaTime
            );
        }
    }
}
