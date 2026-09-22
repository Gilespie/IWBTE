using UnityEngine;

public class TestElevator : MonoBehaviour, IExternalVelocity
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] float _speed = 1f;
    [SerializeField] float _minHeight = 0f;
    [SerializeField] float _maxHeight = 10f;
    public Vector3 ExternalVelocity => Vector3.up * _currentVelocity;
    private float _currentVelocity;

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // платформа не должна падать/реагировать на физику сама
    }

    // вызывается из UnityEvent<float> рычага каждый кадр, пока держите E
    public void SetVelocity(float normalizedInput)
    {
        _currentVelocity = normalizedInput * _speed;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(_currentVelocity) < 0.001f) return;

        float newY = Mathf.Clamp(_rb.position.y + _currentVelocity * Time.fixedDeltaTime, _minHeight, _maxHeight);

        if (Mathf.Approximately(newY, _rb.position.y))
        {
            _currentVelocity = 0f;
            return;
        }

        Vector3 newPos = _rb.position;
        newPos.y = newY;
        _rb.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
        {
            //character.SetExternalVelocity(this);
            character.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
        {
            //character.SetExternalVelocity(null);
            character.transform.SetParent(null);
        }
    }
}