using UnityEngine;

public class Train : MonoBehaviour, IExternalVelocity
{
    [SerializeField] bool _isAutopilot = false;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _acceleration = 5f;
    [SerializeField] float _brakeForce = 5f;
    [SerializeField] float _maxLinearSpeed = 10f;
    [SerializeField] float _drag = 2f;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _trainSound;
    float _currentSpeed = 0f;
    public Vector3 ExternalVelocity => _rb.linearVelocity;

    void FixedUpdate()
    {
        if (_isAutopilot)
        {
            IncrementSpeed();
        }

        ApplyDrag();
        MoveTrain();
        ChangePitch();
    }

    public void IncrementSpeed()
    {
        _currentSpeed += _acceleration * Time.fixedDeltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxLinearSpeed);
    }

    public void DecrementSpeed()
    {
        _currentSpeed -= _brakeForce * Time.fixedDeltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxLinearSpeed);
    }

    void ApplyDrag()
    {
        _currentSpeed -= _drag * Time.fixedDeltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0f, _maxLinearSpeed);
    }

    void MoveTrain()
    {
        _rb.MovePosition(_rb.position + Vector3.right * _currentSpeed * Time.fixedDeltaTime);
        //_rb.linearVelocity = Vector3.right * _currentSpeed;
    }

    public void SetAutopilot()
    {
        _isAutopilot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
        {
            character.SetExternalVelocity(this);
            character.ResetPhysicsMaterial();
            character.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
        {
            character.SetExternalVelocity(null);
            character.ChangePhysicMaterial();
            character.transform.SetParent(null);
        }
    }

    private void ChangePitch()
    {
        var normalizeSpeed = _currentSpeed / _maxLinearSpeed;
        _audioSource.pitch = Mathf.Lerp(1f, 2f, normalizeSpeed);
    }
}