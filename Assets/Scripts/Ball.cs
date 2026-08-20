using UnityEngine;

public class Ball : MonoBehaviour
{
    //[SerializeField] private float _damage = 100f;
    [SerializeField] private GameObject _hitPrefab;
    //[SerializeField] private Player _player;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable dmgeable))
        {
            dmgeable.InstantKill();
        }
    }
}