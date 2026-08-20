using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _cameraBody;
    [SerializeField] float _rotationSpeed = 5f;

    private void Update()
    {
        if (_target == null) return;

        Quaternion targetRotation =
        Quaternion.LookRotation(_target.position - _cameraBody.position);

        _cameraBody.rotation = Quaternion.Slerp(
        _cameraBody.rotation,
        targetRotation,
        _rotationSpeed * Time.deltaTime);
    }
}