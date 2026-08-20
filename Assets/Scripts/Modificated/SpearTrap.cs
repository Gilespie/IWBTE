using Unity.VisualScripting;
using UnityEngine;


public class SpearTrap : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] Vector3 _direction;
    [SerializeField] float _offset = 0.5f;
    Rigidbody _rb;
    bool _isPenetraited = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_isPenetraited) return;

        _isPenetraited = true;

        ContactPoint contact = collision.contacts[0];
        Vector3 penetrationOffset = _direction.normalized * _offset;
        transform.position = contact.point + penetrationOffset;

        transform.SetParent(collision.transform);
        _rb.linearVelocity = Vector3.zero;

        IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.InstantKill();
            collision.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(_direction.normalized * _speed, contact.point, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!_isPenetraited)
        {
            _rb.MovePosition(_rb.position + _direction.normalized * _speed * Time.fixedDeltaTime);
        } 
    }
}