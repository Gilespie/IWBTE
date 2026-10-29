using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    [SerializeField] Transform _point;
    [SerializeField] Vector3 _localOffset = new(0f, 1f, 0f);

    public Vector3 Position => _point != null
        ? _point.position
        : transform.TransformPoint(_localOffset);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Position, 0.1f);
    }
}