using UnityEngine;

public abstract class MovementAdvance : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] protected float _speedMovement;
    [SerializeField] protected float _inputSmoothSpeed = 8f;

    [Header("Acceleration")]
    [SerializeField] protected float _acceleration = 10f;
    [SerializeField] protected float _deceleration = 10f;

    [Header("Jump")]
    [SerializeField] protected float _jumpForce = 5f;

    protected Rigidbody _rb;

    protected Vector3 _smoothedDirection;
    public Vector3 SmoothedDirection => _smoothedDirection;

    protected float _currentSpeed;
    public float CurrentSpeed => _currentSpeed;

    public virtual void Initialize(Rigidbody move)
    {
        _rb = move;
    }

    public abstract void Advance(Vector3 dir, Vector3 externalVelocity);

    public virtual void SetSpeed(float speed)
    {
        _currentSpeed = speed;
    }

    public virtual void Jump()
    {
        /* _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
         _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);*/
        /*Vector3 vel = _rb.linearVelocity;
        vel.y = 0f;
        _rb.linearVelocity = vel;*/
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

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

    protected void UpdateSpeed(bool hasInput)
    {
        float targetSpeed = hasInput ? _speedMovement : 0f;
        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
    }

    protected Vector3 BuildHorizontalVelocity(Vector3 extVelocity)
    {
        return _smoothedDirection.sqrMagnitude > 0.0001f
            ? _smoothedDirection.normalized * _currentSpeed + extVelocity
            : extVelocity;
    }
}