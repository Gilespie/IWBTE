using System.Collections;
using UnityEngine;

public class Climb : MonoBehaviour
{
    [Header("Wall detection (forward)")]
    [SerializeField] float _rayDist = 1.0f;
    [SerializeField] float _rayRadius = 0.5f;
    [SerializeField] LayerMask _climbableLayer;
    [SerializeField] Transform _originPoint;

    [Header("Ledge top detection (downward)")]
    [SerializeField] float _ledgeCheckHeight = 2f;   // откуда начинаем "стрелять" вниз (выше головы)
    [SerializeField] float _ledgeCheckDist = 2.5f;   // как далеко вниз ищем поверхность
    [SerializeField] float _forwardOvershoot = 0.3f; // насколько дальше стены проверяем верх (чтобы не попасть в саму стену)

    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float _climbDuration = 0.8f;
    [SerializeField] AnimationCurve _climbCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] Transform _parent;
    [SerializeField] Rigidbody _rb;
    [SerializeField] CharacterInputController _input;
    [SerializeField] Animator _animator;

    RaycastHit _wallHit;
    RaycastHit _ledgeHit;
    bool _isHitted = false;
    bool _isClimbing = false;

    void Update()
    {
        if (_isClimbing) return;

        if (IsRaycasting(transform.forward))
        {
            if (_input.IsClimbing && TryFindLedge(out Vector3 climbTarget))
            {
                ClimbPlayer(climbTarget);
            }
        }

        _input.ResetClimb();
    }

    bool IsRaycasting(Vector3 direction)
    {
        Ray ray = new Ray(_originPoint.position, direction);
        return _isHitted = Physics.SphereCast(ray, _rayRadius, out _wallHit, _rayDist, _climbableLayer);
    }

    // Динамически ищем верхнюю точку уступа
    bool TryFindLedge(out Vector3 target)
    {
        target = default;

        // Точка чуть за стеной (по горизонтали), чтобы луч сверху точно упал 
        // на верхнюю поверхность, а не задел саму вертикальную стену
        Vector3 horizontalPoint = _wallHit.point + _parent.forward * _forwardOvershoot;

        Vector3 downOrigin = new Vector3(horizontalPoint.x, _parent.position.y + _ledgeCheckHeight, horizontalPoint.z);

        if (Physics.Raycast(downOrigin, Vector3.down, out _ledgeHit, _ledgeCheckDist, _climbableLayer))
        {
            // Высота, на которую реально нужно подняться
            float climbHeight = _ledgeHit.point.y - _parent.position.y;

            // Если уступ слишком низкий или слишком высокий — не считаем его "залезаемым"
            if (climbHeight < 0.3f || climbHeight > _ledgeCheckHeight)
                return false;

            // Целевая позиция: сама точка на поверхности уступа,
            // чуть сдвинутая назад от края, чтобы не зависать на самом краю
            target = _ledgeHit.point - _parent.forward * 0.1f;
            return true;
        }

        return false;
    }

    void ClimbPlayer(Vector3 targetPos)
    {
        _rb.isKinematic = true;
        _input.DisableAllInput();
        _animator.CrossFade(AnimParams.Climb, 0.1f);

        StartCoroutine(ClimbRoutine(targetPos));
    }

    IEnumerator ClimbRoutine(Vector3 targetPos)
    {
        _isClimbing = true;

        Vector3 startPos = _parent.position;

        // Промежуточная точка — "зацеп" руками на краю уступа,
        // чтобы траектория не была прямой линией сквозь стену
        Vector3 midPos = new Vector3(startPos.x, targetPos.y - 0.1f, startPos.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _climbDuration;
            float eased = _climbCurve.Evaluate(t);

            // Двухфазное движение: сначала вверх до уровня уступа (0-0.6),
            // потом вперёд на саму поверхность (0.6-1)
            if (eased < 0.6f)
            {
                float localT = eased / 0.6f;
                _parent.position = Vector3.Lerp(startPos, midPos, localT);
            }
            else
            {
                float localT = (eased - 0.6f) / 0.4f;
                _parent.position = Vector3.Lerp(midPos, targetPos, localT);
            }

            yield return null;
        }

        _parent.position = targetPos;

        _rb.isKinematic = false;
        _input.EnableMovementMap();

        _isClimbing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isHitted ? Color.green : Color.red;
        Gizmos.DrawRay(_originPoint.position, transform.forward * _rayDist);
        Gizmos.DrawWireSphere(_originPoint.position + transform.forward * _rayDist, _rayRadius);

        if (Application.isPlaying && _isHitted)
        {
            Vector3 horizontalPoint = _wallHit.point + transform.forward * _forwardOvershoot;
            Vector3 downOrigin = new Vector3(horizontalPoint.x, transform.position.y + _ledgeCheckHeight, horizontalPoint.z);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(downOrigin, Vector3.down * _ledgeCheckDist);
        }
    }
}