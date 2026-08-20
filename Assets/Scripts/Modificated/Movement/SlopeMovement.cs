using UnityEngine;

public class SlopeMovement : MovementAdvance
{
    [SerializeField] SlopeRaycast _slopeRaycast;
    [SerializeField] float _maxSlideSpeed = 6f;

    public override void Advance(Vector3 dir, Vector3 extVelocity)
    {
        if (!_slopeRaycast.IsRaycasting(-Vector3.up)) return;

        /*Vector3 slopeNormal = _slopeRaycast.Normal;

        _direction = transform.forward * dir.z;
        _rb.MovePosition(_rb.position + _direction.normalized * _speedMovement * Time.fixedDeltaTime);*/

        Vector3 direction = transform.forward * dir.z;
        _rb.MovePosition(_rb.position + direction.normalized * _speedMovement * Time.fixedDeltaTime);

        /* Vector3 slopeDir = Vector3.ProjectOnPlane(
             new Vector3(dir.x, 0f, dir.z),
             slopeNormal
         ).normalized;

         _rb.linearVelocity = slopeDir * _speedMovement;*/
    }
}