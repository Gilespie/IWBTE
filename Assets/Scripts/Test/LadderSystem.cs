using UnityEngine;

public class LadderSystem : MonoBehaviour
{
    [SerializeField] private Raycasting _raycasting;
    [SerializeField] private float _ladderSpeed = 2f;
    [SerializeField] private string _triggerName = "onLadder";
    [SerializeField] private string _triggerOutName = "onOutLadder";
    private bool _isOnLadder = false;
    private Vector3 _ladderDir;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _ladderDir.y = Input.GetAxis("Jump") > 0 ? 1 : (Input.GetKey(KeyCode.LeftControl) ? -1 : 0);
        _animator.SetFloat("yAxis", _ladderDir.y);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!_isOnLadder && _raycasting.IsInteract())
            {
                SetOnLadder(true);
                _animator.SetTrigger(_triggerName);
            }
            else if(_isOnLadder)
            {
                SetOnLadder(false);
                _animator.SetTrigger(_triggerOutName);
            }
        }
    }

    private void FixedUpdate()
    {
        if(_ladderDir.y != 0f && _isOnLadder)
        {
            MoveToLadder(_ladderDir);
        }
    }

    private void MoveToLadder(Vector3 dir)
    {
        _rigidbody.MovePosition(transform.position + dir * _ladderSpeed * Time.fixedDeltaTime);
    }

    public void SetOnLadder(bool value)
    {
        _isOnLadder = value;
        _rigidbody.useGravity = !value; // ���� �� �������� � ���������� ����
        _rigidbody.linearVelocity = Vector3.zero; // ����� �������� ��� ��������/����������
    }
}