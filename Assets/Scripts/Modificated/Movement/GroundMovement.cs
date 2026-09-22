using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody _rb;

    [Header("Speed")]
    [SerializeField] float _walkSpeed = 3f;
    [SerializeField] float _runSpeed = 5f;
    [SerializeField] float _sprintSpeed = 8f;
    [SerializeField] float _pushSpeed = 1f;
    [SerializeField] float _crouchSpeed = 3f;
    [SerializeField] float _swimSpeed = 2f;
    [SerializeField] float _maxSlideSpeed = 6f;

    [SerializeField] float _inputSmoothSpeed = 8f;

    [Header("Acceleration")]
    [SerializeField] float _acceleration = 10f;
    [SerializeField] float _deceleration = 10f;

    [Header("Jump")]
    [SerializeField] float _jumpForce = 5f;

    float _currentSpeed;
    Vector3 _smoothedDirection;

    public float CurrentSpeed => _currentSpeed;
    public Vector3 SmoothedDirection => _smoothedDirection;

    void UpdateSpeed(bool hasInput, float speed)
    {
        float targetSpeed = hasInput ? speed : 0f;
        float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
    }

    public void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    public void Crouch(Vector3 dir)
    {
        SetDirectionAndSpeed(new(dir.x, 0, dir.z), _crouchSpeed);
    }

    public void Sprint(Vector3 dir)
    {
        SetDirectionAndSpeed(new(dir.x, 0, dir.z), _sprintSpeed);
    }

    public void Walk(Vector3 dir)
    {
        SetDirectionAndSpeed(new(dir.x, 0, dir.z), _walkSpeed);
    }
    public void Run(Vector3 dir)
    {
        SetDirectionAndSpeed(new(dir.x,0,dir.z), _runSpeed);
    }

    public void Slide(Vector3 dir)
    {
        SetDirectionAndSpeed(transform.forward * dir.z, _maxSlideSpeed);
    }

    public void Push(Vector3 dir, PushableBox box)
    {
        if (box == null) return;
        SetDirectionAndSpeed(new(dir.x, 0, dir.z), _pushSpeed);
        box.Rb.MovePosition(box.Rb.position + new Vector3(_smoothedDirection.x, 0f, _smoothedDirection.z) * _currentSpeed * Time.fixedDeltaTime);
    }

    public void Swimming(Vector3 dir)
    {
        SetDirectionAndSpeed(dir, _swimSpeed);
    }

    private void SetDirectionAndSpeed(Vector3 dir, float speed)
    {
        _smoothedDirection = new Vector3(dir.x, dir.y, dir.z);
        
        UpdateSpeed(_smoothedDirection.sqrMagnitude > 0.01f, speed);

        Vector3 delta = _smoothedDirection.sqrMagnitude > 1f ? _smoothedDirection.normalized * _currentSpeed : _smoothedDirection * _currentSpeed;
        _rb.MovePosition(_rb.position + delta * Time.fixedDeltaTime);
    }
}