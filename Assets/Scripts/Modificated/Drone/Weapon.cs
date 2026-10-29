using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /* [SerializeField] private Bullet _projectilePrefab;
     [SerializeField] private Transform _firePoint;
     [SerializeField] Transform _weaponMesh;
     [SerializeField] AudioClip _shootClip;
     [SerializeField] AudioSource _audioSource;

     [SerializeField] private float _fireRate = 1f;
     [SerializeField] float _aimDelay = 0.6f;
     [SerializeField] float _spread = 0.04f;

     [Header("Rotation")]
     [SerializeField] float _turnSpeed = 240f;
     [SerializeField] float _aimTolerance = 5f;

     Transform _target;
     AimTargetStruct _aim;
     Quaternion _restLocalRotation;
     bool _isTracking;
     float _nextShootTime;
     float _aimReadyTime;

     void Awake() => _restLocalRotation = _weaponMesh.localRotation;

     void Update() => UpdateRotation();

     public void SetTracking(bool value)
     {
         if (value && !_isTracking) _aimReadyTime = Time.time + _aimDelay;
         _isTracking = value;
     }
     public void SetTarget(Transform target)
     {
         _target = target;
         _aim = new AimTargetStruct(target);
     }

     public void ResetAim() => _aimReadyTime = Time.time + _aimDelay;

     public bool TryShoot()
     {
         if (!_isTracking || _target == null) return false;
         if (Time.time < _aimReadyTime || Time.time < _nextShootTime) return false;

         Vector3 dir = GetAimPoint() - _firePoint.position;
         if (Vector3.Angle(_firePoint.forward, dir) > _aimTolerance) return false;

         Fire(_firePoint.forward);
         _nextShootTime = Time.time + 1f / _fireRate;
         return true;
     }

     void UpdateRotation()
     {
         Quaternion desired;

         if (_isTracking && _target != null)
         {
             Vector3 dir = GetAimPoint() - _weaponMesh.position;
             if (dir.sqrMagnitude < 0.001f) return;
             desired = Quaternion.LookRotation(dir);
         }
         else
         {
             Transform parent = _weaponMesh.parent;
             desired = parent != null ? parent.rotation * _restLocalRotation : _restLocalRotation;
         }

         _weaponMesh.rotation = Quaternion.RotateTowards(_weaponMesh.rotation, desired, _turnSpeed * Time.deltaTime);
     }

     *//*void Fire(Vector3 dir)
     {
         dir = (dir + Random.insideUnitSphere * _spread).normalized;

         Bullet projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.LookRotation(dir));
         projectile.Launch(dir);
         _audioSource.PlayOneShot(_shootClip);
     }
 *//*
     void Fire(Vector3 dir)
     {
         dir = (dir + Random.insideUnitSphere * _spread).normalized;

         Bullet projectile = Instantiate(
             _projectilePrefab,
             _firePoint.position,
             Quaternion.LookRotation(dir)
         );

         projectile.Launch(dir, _aim);

         _audioSource.PlayOneShot(_shootClip);

     }

     Vector3 GetAimPoint() => _aim.Position;*/

    [Header("References")]
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _weaponMesh;
    [SerializeField] AudioClip _shootClip;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GameObject _flashPrefab;  
    [SerializeField] float _flashDuration = 0.1f;            

    [Header("Fire")]
    [SerializeField] float _fireRate = 1f;
    [SerializeField] float _aimDelay = 0.6f;            
    [SerializeField] float _aimTolerance = 5f;          
    [SerializeField] bool _perfectAim = true;           
    [SerializeField] float _spread = 0.04f;             

    [Header("Hitscan")]
    [SerializeField] LayerMask _hitMask;                
    [SerializeField] float _range = 50f;
    [SerializeField] float _hitRadius = 0.1f;           

    [Header("Rotation")]
    [SerializeField] float _turnSpeed = 240f;

    [Header("Impact")]
    [SerializeField] ParticleSystem _impactPrefab;          
    [SerializeField] ParticleSystem _damageImpactPrefab;
    [SerializeField] float _impactLifetime = 3f;
    [SerializeField] float _impactSurfaceOffset = 0.01f;

    Coroutine _routine;
    Transform _target;
    AimTargetStruct _aim;
    Quaternion _restLocalRotation;
    bool _isTracking;
    float _nextShootTime;
    float _aimReadyTime;

    void Awake()
    {
        _restLocalRotation = _weaponMesh.localRotation;
        _flashPrefab.SetActive(false);
    }

    void Update() => UpdateRotation();

    public void SetTracking(bool value)
    {
        if (value && !_isTracking) _aimReadyTime = Time.time + _aimDelay;
        _isTracking = value;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _aim = new AimTargetStruct(target);
    }

    public void ResetAim() => _aimReadyTime = Time.time + _aimDelay;

    public bool TryShoot()
    {
        if (!_isTracking || _target == null) return false;
        if (Time.time < _aimReadyTime || Time.time < _nextShootTime) return false;

        Vector3 toTarget = GetAimPoint() - _firePoint.position;
        if (Vector3.Angle(_firePoint.forward, toTarget) > _aimTolerance) return false;

        Vector3 dir = _perfectAim ? toTarget.normalized : _firePoint.forward;
        Fire(dir);

        _nextShootTime = Time.time + 1f / _fireRate;
        return true;
    }

    void UpdateRotation()
    {
        Quaternion desired;

        if (_isTracking && _target != null)
        {
            Vector3 dir = GetAimPoint() - _weaponMesh.position;
            if (dir.sqrMagnitude < 0.001f) return;
            desired = Quaternion.LookRotation(dir);
        }
        else
        {
            Transform parent = _weaponMesh.parent;
            desired = parent != null ? parent.rotation * _restLocalRotation : _restLocalRotation;
        }

        _weaponMesh.rotation = Quaternion.RotateTowards(
            _weaponMesh.rotation, desired, _turnSpeed * Time.deltaTime);
    }

    void Fire(Vector3 dir)
    {
        if (!_perfectAim)
            dir = (dir + Random.insideUnitSphere * _spread).normalized;

        Vector3 origin = _firePoint.position;

        bool hitSomething = _hitRadius > 0f
            ? Physics.SphereCast(origin, _hitRadius, dir, out RaycastHit hit, _range, _hitMask, QueryTriggerInteraction.Ignore)
            : Physics.Raycast(origin, dir, out hit, _range, _hitMask, QueryTriggerInteraction.Ignore);

        if (hitSomething)
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            bool hitDamageable = dmg != null && dmg.IsAlive;

            if (hitDamageable)
                dmg.InstantKill();

            SpawnImpact(hit, hitDamageable);
        }

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(FlashRoutine());

        if (_audioSource != null && _shootClip != null)
            _audioSource.PlayOneShot(_shootClip);
    }

    void SpawnImpact(RaycastHit hit, bool hitDamageable)
    {
        ParticleSystem prefab = hitDamageable && _damageImpactPrefab != null
            ? _damageImpactPrefab
            : _impactPrefab;

        if (prefab == null) return;

        Vector3 pos = hit.point + hit.normal * _impactSurfaceOffset;
        Quaternion rot = Quaternion.LookRotation(hit.normal);

        ParticleSystem impact = Instantiate(prefab, pos, rot);
        impact.Play();

        Destroy(impact.gameObject, _impactLifetime);
    }

    Vector3 GetAimPoint() => _aim.Position;

    IEnumerator FlashRoutine()
    {
        if (_flashPrefab != null)
        {
            _flashPrefab.SetActive(true);
            yield return new WaitForSeconds(_flashDuration);
            _flashPrefab.SetActive(false);
        }
    }
}