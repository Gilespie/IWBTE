using UnityEngine;

public class Bullet : MonoBehaviour
{ 
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _turnSpeed = 360f;
    [SerializeField] private float _damage = 1000f;
    [SerializeField] Rigidbody _rb;
    Vector3 _dir;
    AimTargetStruct _aim;
    bool _launched;

    public void Launch(Vector3 direction, AimTargetStruct aim)
    {
        _dir = direction.normalized;
        _aim = aim;
        _launched = true;

        transform.forward = _dir;

        Destroy(gameObject, _lifetime);
    }

    void FixedUpdate()
    {
        if (!_launched || !_aim.IsValid) return; 

        Vector3 aimPosition = _aim.Position; 
        Vector3 directionToTarget = aimPosition - _rb.position; 
        
        if (directionToTarget.sqrMagnitude > 0.001f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
            Quaternion newRotation = Quaternion.RotateTowards( _rb.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime ); 
            _rb.MoveRotation(newRotation); _dir = newRotation * Vector3.forward;
        }
        
        _rb.MovePosition( _rb.position + _dir * _speed * Time.fixedDeltaTime ); 
    }

    void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.InstantKill();
        }

        Destroy(gameObject);
    }
}