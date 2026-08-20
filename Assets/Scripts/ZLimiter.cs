using UnityEngine;

public class ZLimiter : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [Header("Z Limits")]
    [SerializeField] private float _zPosMin = -5f;
    [SerializeField] private float _zPosMax = 2.34f;
    private float _zPosMaxDefault;

    private void Awake()
    {
        _zPosMaxDefault = _zPosMax;
    }

    private void FixedUpdate()
    {
        CheckZPosition();
    }

    private void CheckZPosition()
    {
        Vector3 pos = _rb.position;
        pos.z = Mathf.Clamp(pos.z, _zPosMin, _zPosMax);
        _rb.MovePosition(pos);
    }

    public void SetZMaxLimit(float value)
    {
        _zPosMax = value;
    }

    public void ResetLimit()
    {
        _zPosMax = _zPosMaxDefault;
    }
}