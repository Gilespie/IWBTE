using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _speedFly = 55f;
    [SerializeField] float _radius = 5f;
    [SerializeField] float _damage = 500f;
    [SerializeField] float _forceExplosion = 50f;
    [SerializeField] float _forceUpExplosion = 5f;
    [SerializeField] float _stopDistance = 1f;
    [SerializeField] LayerMask _player;
    [SerializeField] float _timeToDeactivate = 5f;

    [Header("Audio")]
    [SerializeField] AudioSource _audiosource;
    [SerializeField] AudioClip _flyClip;

    [Header("References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] GameObject _mesh;
    [SerializeField] Collider _col;
    [SerializeField] ParticleSystem _trailParticle;
    Transform _target;
    Vector3 _direction;
    bool _isExploded;

    private void Awake()
    {
        _audiosource.clip = _flyClip;
        _audiosource.Play();
    }

    private void FixedUpdate()
    {
        if (_isExploded || _target == null)
            return;

        Vector3 dir = _target.position - transform.position;

        float step = _speedFly * Time.fixedDeltaTime;
        float dist = dir.magnitude;

        if (dist <= Mathf.Max(_stopDistance, step))
        {
            Explode(_target.position);
            return;
        }

        _direction = dir.normalized;

        Quaternion view = Quaternion.LookRotation(_direction);

        _rb.MoveRotation(view);
        _rb.MovePosition(_rb.position + _direction * _speedFly * Time.fixedDeltaTime);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    void Explode(Vector3 point)
    {
        Explosion(point);

        Instantiate(_explosionPrefab, point, _explosionPrefab.transform.rotation);

        CameraShake.Instance.ActiveShake();

        _mesh.SetActive(false);
        _col.enabled = false;
        _trailParticle.Stop();

        _isExploded = true;
        StartCoroutine(WaitParticles());
    }

    private void Explosion(Vector3 centre)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _player);

        foreach (Collider col in colliders)
        {
            Destructable dest = col.GetComponent<Destructable>();

            if (dest != null)
            {
                dest.InstantKill();

                Rigidbody[] rb = col.GetComponentsInChildren<Rigidbody>();

                foreach(Rigidbody rb2 in rb)
                {
                    rb2.AddExplosionForce(_forceExplosion, centre, _radius, _forceUpExplosion, ForceMode.Impulse);
                }
            }
        }
    }

    IEnumerator WaitParticles()
    {
        yield return new WaitForSeconds(_timeToDeactivate);
        gameObject.SetActive(false);
    }
}