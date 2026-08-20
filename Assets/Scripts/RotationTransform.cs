using UnityEngine;

public class RotationTransform : MonoBehaviour
{
    [SerializeField] float _speedRotDefault = 10f;
    [SerializeField] float _speedRotPushing = 3f;
    private float _currentRotationSpeed = 0f;
    private Vector3 _dir;
    private Quaternion _rot;
    private float x;
    private float y;
    private bool _isPushing = false;


    private void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        _dir = new Vector3(x, 0, y);

        if (_dir.magnitude >= 0.1f)
        {
            if (_isPushing)
            {
                ChangeRotateSpeed(_speedRotPushing);
                RotateTransform();
            }
            else
            {
                ChangeRotateSpeed(_speedRotDefault);
                RotateTransform();
            }       
        }
    }

    private void RotateTransform()
    {
        
        _rot = Quaternion.LookRotation(_dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, _rot, Time.deltaTime * _currentRotationSpeed);
    }

    public void ChangeRotateSpeed(float speed)
    {
        _currentRotationSpeed = speed;
    }
}