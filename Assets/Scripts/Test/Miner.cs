using UnityEngine;

[ExecuteAlways]
public class Miner : MonoBehaviour
{
    [Header("Animation triggers")]
    [SerializeField] bool _deadlyPain = false;
    [SerializeField] bool _tremor = false;
    [SerializeField] bool _die1 = false;
    [SerializeField] bool _dieFront1 = false;
    [SerializeField] bool _dieFront2 = false;
    [SerializeField] bool _pose = false;
    [SerializeField] bool _pose2 = false;

    Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (_deadlyPain) _animator.Play("DeadlyPain");
        else if (_tremor) _animator.Play("Tremor");
        else if (_die1) _animator.Play("Die");
        else if (_dieFront1) _animator.Play("DieFront");
        else if (_dieFront2) _animator.Play("DieFront2");
        else if (_pose) _animator.Play("Pose1");
        else if (_pose2) _animator.Play("Pose2");
    }
}