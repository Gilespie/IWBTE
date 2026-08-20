using UnityEngine;

public class WagoonMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    public Transform[] targets;
    public float offset = 0.1f;
    private Transform _currentTarget;
    private int index;
    private Vector3 dir;
    private float distance;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _currentTarget = targets[0];
    }

    private void Update()
    {
        dir = (_currentTarget.position - transform.position).normalized;

        distance = (_currentTarget.position - transform.position).sqrMagnitude;

        if (distance <= offset)
        {
            _currentTarget = SetNextTarget();
        }
    }

    void FixedUpdate()
    {
       rb.MovePosition(transform.position + dir * speed * Time.fixedDeltaTime);
    }

    private Transform SetNextTarget()
    {
        index++;
        if (index >= targets.Length) index = 0;

        _currentTarget = targets[index];

        return _currentTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
            player.InstantKill();
    }
}
