using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] protected string _nameIdle = "Vert"; 
    [SerializeField] protected string _nameRun = "State"; 

    [Header("Enemy Settings")]
    //[SerializeField] protected float damage = 100f;
    [SerializeField] protected float attackDistance = 2f;
    [SerializeField] protected float detectionDistance = 5f;
    [SerializeField] protected float _updateNodeDistance = 0.75f;

    [Header("Audio")]
    [SerializeField] protected AudioClip _idleClip;
    [SerializeField] protected AudioClip _activeClip;
    [SerializeField] protected Vector2 _pitchRange = new Vector2(0.9f, 1.1f);
    protected AudioSource _audioSource;

    protected Transform[] _aiNodes;
    protected Transform _actualNode;
    protected float _distanceToPlayer, _distanceToNode;
    protected Animator _animator;
    protected Rigidbody _rb;
    protected Character _player;
    protected Vector3 _direction;
    protected NavMeshAgent _agent;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        _player = GameManager.Instance.Player;
        //_aiNodes = GameManager.Instance.AIDogNodes;

        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);
    }

    protected virtual void Update()
    {
        if (_player == null) return;

        _animator.SetFloat(_nameIdle, _agent.velocity.magnitude);
        _animator.SetFloat(_nameRun, _agent.velocity.magnitude);

        _distanceToPlayer = Vector3.SqrMagnitude(transform.position - _player.transform.position);

        if (_distanceToPlayer <= detectionDistance * detectionDistance && _player.IsAlive)
        {
            PlaySound(_activeClip);
            //Bark();
            Act();
        }
        else
        {
            PlaySound(_idleClip);
            //Growl();
            Idle();
        }
    }

    protected virtual void Act()
    {

    }    

    protected virtual void Idle()
    {

        if (_agent.destination != _actualNode.position)
        {
            _agent.SetDestination(_actualNode.position);
        }
        
        _distanceToNode = Vector3.SqrMagnitude(transform.position - _actualNode.position);

        if (_distanceToNode <= _updateNodeDistance * _updateNodeDistance)
        {
            _actualNode = GetNewNode(_actualNode);
            _agent.SetDestination(_actualNode.position);
        }
    }

    protected void DealDamage(IDamageable destructable)
    {
        destructable.InstantKill();
    }

    private Transform GetNewNode(Transform actual = null)
    {
        if(!actual)
        {
            return _aiNodes[Random.Range(0, _aiNodes.Length)];
        }
        else
        {
            Transform newNode;

            do
            {
                newNode = _aiNodes[Random.Range(0, _aiNodes.Length)];
            }
            while (actual == newNode);

            return newNode;
        }
    }

    protected void PlaySound(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;

        if (_audioSource.clip != clip || !_audioSource.isPlaying)
        {
            _audioSource.clip = clip;
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.Play();
        }
    }

    protected void Growl()
    {
        if (_audioSource != null && _idleClip != null && !_audioSource.isPlaying)
        {
            _audioSource.clip = _idleClip;
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.Play();
        }
    }

    protected void Bark()
    {
        if (_audioSource != null && _activeClip != null && !_audioSource.isPlaying)
        {
            _audioSource.clip = _activeClip;
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.Play();
        }
    }
}