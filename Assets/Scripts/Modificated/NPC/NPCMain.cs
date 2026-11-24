using UnityEngine;

public class NPCMain : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Rigidbody _rigidbody;

    [Header("Moving")]
    [SerializeField] float _speedWalk = 1f;
    [SerializeField] float _speedRun = 2f;
    [SerializeField] float _speedRotation = 1f;
    [SerializeField] Vector3 _dir;
    [SerializeField] bool _isRunning = false;

    [Header("Anim")]
    [SerializeField] bool _hasAction = false;
    [SerializeField] float _index = 0;
    [SerializeField] string _triggerName = "Action";
    [SerializeField] string _indexName = "Index";
    [SerializeField] string _speedName = "Speed";

    [Header("SFX")]
    [SerializeField] AudioClip[] _voiceSFX;
    [SerializeField] bool _avoidRepeat = true;
    int _lastVoiceIndex = -1;
    float CurrentSpeed => _isRunning ? _speedRun : _speedWalk;

    void Start()
    {
        if (_hasAction)
        {
            PlayAction();
        }
    }

    void Update()
    {
        bool hasDir = _dir.sqrMagnitude > 0.0001f;

        _animator.SetFloat(_speedName, hasDir ? CurrentSpeed : 0f);
    }

    void FixedUpdate()
    {
        if (_dir.sqrMagnitude < 0.0001f) return;

        Vector3 moveDir = _dir.normalized;

        _rigidbody.MovePosition(_rigidbody.position + moveDir * CurrentSpeed * Time.fixedDeltaTime);

        RotateToDirection(moveDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DronBase dron))
        {
            gameObject.SetActive(false);
        }
    }

    void RotateToDirection(Vector3 dir)
    {
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        _rigidbody.MoveRotation(
            Quaternion.Slerp(_rigidbody.rotation, targetRotation, _speedRotation * Time.fixedDeltaTime));
    }

    public void MakePanic()
    {
        SetDirection(-_dir);
        SetRunning(true);
        PlayRandomVoice();
    }

    public void SetDirection(Vector3 dir)
    {
        _dir = dir;
    }

    public void SetRunning(bool value)
    {
        _isRunning = value;
    }

    public void PlayAction()
    {
        _animator.SetTrigger(_triggerName);
        _animator.SetFloat(_indexName, _index);
    }

    public void MakeWalk()
    {
        SetRunning(false);
        _dir = Vector3.right;
    }

    public void PlayRandomVoice()
    {
        if (_voiceSFX == null || _voiceSFX.Length == 0 || _audioSource == null) return;

        int index = Random.Range(0, _voiceSFX.Length);

        if (_avoidRepeat && _voiceSFX.Length > 1)
        {
            while (index == _lastVoiceIndex)
                index = Random.Range(0, _voiceSFX.Length);
        }

        _lastVoiceIndex = index;

        AudioClip clip = _voiceSFX[index];

        if (clip == null) return;

        _audioSource.PlayOneShot(clip);
    }
}