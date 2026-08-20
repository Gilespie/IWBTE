using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Header("Soldier Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioClip _shootSFX;
    [SerializeField] private Transform[] viewDirections;
    [SerializeField] private float rotationInterval = 3f;
    [SerializeField] private Player _player;

    private int _currentDirectionIndex = 0;
    private float _rotationTimer;
    private float _fireTimer;
    private AudioSource _audioSource;
    private Quaternion _targetRotation;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;
        _rotationTimer += Time.deltaTime;

        if (_player != null && _player.IsAlive && CanSeePlayer())
        {
            Act();
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        if (_rotationTimer >= rotationInterval)
        {
            _currentDirectionIndex = (_currentDirectionIndex + 1) % viewDirections.Length;
            Vector3 targetDirection = (viewDirections[_currentDirectionIndex].position - transform.position).normalized;
            targetDirection.y = 0f;

            if (targetDirection.sqrMagnitude > 0.01f)
            {
                _targetRotation = Quaternion.LookRotation(targetDirection);
            }

            _rotationTimer = 0;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 2f); // 2f — скорость, можно настроить
    }

    private void Act()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // Стрельба
        if (_fireTimer >= fireRate)
        {
            _fireTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        _audioSource.PlayOneShot(_shootSFX);

        if(bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().AssingPlayer(_player);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 dirToPlayer = _player.transform.position - transform.position;
        if (dirToPlayer.sqrMagnitude > visionRange * visionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
        if (angle > visionAngle)
            return false;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer.normalized, out RaycastHit hit, visionRange))
        {
            if (hit.collider.GetComponent<Destructable>() == _player)
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        // Центр зрения
        Vector3 origin = transform.position + Vector3.up;

        // Главный вектор (вперед)
        Vector3 forward = transform.forward * visionRange;

        // Границы угла обзора
        Quaternion leftRotation = Quaternion.AngleAxis(-visionAngle, Vector3.up);
        Quaternion rightRotation = Quaternion.AngleAxis(visionAngle, Vector3.up);

        Vector3 leftBoundary = leftRotation * transform.forward * visionRange;
        Vector3 rightBoundary = rightRotation * transform.forward * visionRange;

        // Рисуем линии
        Gizmos.DrawRay(origin, forward);         // Прямо
        Gizmos.DrawRay(origin, leftBoundary);    // Левая граница
        Gizmos.DrawRay(origin, rightBoundary);   // Правая граница

        // Можно добавить дугу (опционально, чуть сложнее)
        DrawViewArc(origin, visionRange, visionAngle, 30);
    }

    private void DrawViewArc(Vector3 origin, float radius, float angle, int segments)
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f); // Полупрозрачный жёлтый

        float startAngle = -angle;
        float angleStep = (angle * 2f) / segments;

        Vector3 prevPoint = origin + Quaternion.Euler(0, startAngle, 0) * transform.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector3 nextPoint = origin + Quaternion.Euler(0, currentAngle, 0) * transform.forward * radius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}