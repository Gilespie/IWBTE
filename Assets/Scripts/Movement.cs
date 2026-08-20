using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _speedMultiplier = 1f;
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _crouchSpeed = 5f;
    [SerializeField] private float _pushingSpeed = 1f;
    [SerializeField] private float _jumpForce = 25f;
    [SerializeField] private Raycasting _raycasting;

    private float _currentSpeed = 0f;
    private Vector3 _direction;
    private Rigidbody _rb;
    private CapsuleCollider _col;
    
    private bool _isGrounded = false;
    private bool _isCrouch = false;
    private bool _isPushing = false;
    private bool _isOnce = false;
    private bool _canMove = false;
    public bool CanMove => _canMove;

    private bool _isInteractable = false;
    private bool _isSlope = false;
    private bool _isCeiling = false;


    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
    }

    protected void Update()
    {
        _isGrounded = _raycasting.IsGrounded();
            //    _direction.x = Input.GetAxis("Horizontal");
    //    _direction.z = Input.GetAxis("Vertical");

    //    if (_isSlope)
    //    {
    //        _animator.SetBool(_slopeBoolName, _isSlope);

    //        _rb.maxLinearVelocity = 15f;
    //        _col.sharedMaterial = _physicsMaterial;
    //        SlopeRotateMesh();
    //    }
    //    else
    //    {
    //        _animator.SetBool(_slopeBoolName, _isSlope);
    //        _rb.maxLinearVelocity = float.MaxValue;
    //        _col.sharedMaterial = null;
    //    }

    //    CalculateFallDamage();

    //    if (Input.GetKeyDown(_jumpKey) && _isGrounded && !_isCrouch && !_isSlope)
    //    {
    //        _animator.SetTrigger(_jumpTriggerName);
    //        JumpPlayer();
    //    }

    //    if (Input.GetKeyDown(_pressingKey) && _isGrounded && _isInteractable) //old version eliminar en el futuro
    //    {
    //        _animator.SetTrigger(_pressTriggerName);
    //        Pressing();
    //    }

    //    if (Input.GetKeyDown(_crouchKey) && _isGrounded && !_isCeiling)
    //    {
    //        _isCrouch = !_isCrouch;
    //    }

    //    /*        if (Input.GetKeyDown(_pushingKey) && _isGrounded && _isInteractable)  new version hacer asi
    //            {
    //                _animator.SetTrigger(_pressTriggerName);
    //                Pressing();
    //            }*/

    //    if (Input.GetKey(_pushingKey) && _isGrounded && _isPushing && !_isCrouch)
    //    {
    //        if (!_canMove)
    //        {
    //            Pushing();
    //        }
    //    }
    //    else
    //    {
    //        if (_canMove)
    //        {
    //            StopPushing();
    //        }
    //    }

    //    if (_isCrouch)
    //    {
    //        Crouch();
    //    }
    //    else
    //    {
    //        Uncrouch();
    //    }

    //    if (_canMove)
    //    {
    //        ChangeSpeed(_pushingSpeed);
    //    }
    //    else if (_isCrouch)
    //    {
    //        ChangeSpeed(_crouchSpeed);
    //    }
    //    else
    //    {
    //        ChangeSpeed(_moveSpeed);
    //    }
    }
    public float ChangeSpeed(float speed)
    {
        _currentSpeed = speed * _speedMultiplier;
        return _currentSpeed;
    }

    public void SetSpeedMultiplier(float value)
    {
        _speedMultiplier = value;
    }

    public void MovePlayer(Vector3 dir)
    {
        Vector3 moveDir = new( );

        if (_isSlope)
        {
             moveDir = (transform.forward * dir.z).normalized;
        }
        else
        {
            moveDir = (transform.right * dir.x + transform.forward * dir.z).normalized;
        }

        _rb.MovePosition(transform.position + moveDir * _currentSpeed * Time.fixedDeltaTime);
    }

    public void SlopeMovement()
    {
        Vector3 moveDir = (transform.forward * _direction.z).normalized;
        _rb.MovePosition(transform.position + moveDir * _currentSpeed * Time.fixedDeltaTime);
    }

    public void SetMovement(float x, float z, float jumpForce, float speed) //abstract method for movement
    {
        _direction.x = x;
        _direction.z = z;

        if (jumpForce > 0)
        {
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        Vector3 moveDir = (transform.right * _direction.x + transform.forward * _direction.z).normalized;
        _rb.MovePosition(transform.position + moveDir * speed * Time.fixedDeltaTime);
    }

    public void SetMovement(float x, float z, float speed) //abstract method for movement
    {
        _direction.x = x;
        _direction.z = z;

        Vector3 moveDir = (transform.right * _direction.x + transform.forward * _direction.z).normalized;
        _rb.MovePosition(transform.position + moveDir * speed * Time.fixedDeltaTime);
    }

    public float GetSpeed()
    {
        return _currentSpeed;
    }

    public void JumpPlayer()
    {
        if (!_isGrounded) return;

        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

        Vector3 moveDir = (transform.forward * _direction.z).normalized;
        _rb.MovePosition(transform.position + moveDir * _currentSpeed * Time.fixedDeltaTime);
    }

    public void Crouch()
    {
        _col.height = 1;
        _col.center = new Vector3(_col.center.x, 0.5f, _col.center.z);
    }

    public void Uncrouch()
    {
        _col.height = 2;
        _col.center = new Vector3(_col.center.x, 1f, _col.center.z);
    }

    public void Pushing()
    {
        //if (_canMove) return;

        //_canMove = true;
        //_raycast.Interact();
        //_raycast.Pushing();
        //_rotationTransform.IsPushing(_canMove);
    }

    public void StopPushing()
    {
        //_canMove = false;
        //_raycast.Interact();
        //_raycast.Pushing();
        //_rotationTransform.IsPushing(_canMove);
    }
}