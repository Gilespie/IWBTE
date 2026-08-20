using UnityEngine;

public class WindTurbin : MonoBehaviour
{
    [SerializeField] private string _triggerName = "onExplosion";
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetTrigger()
    {
        _animator.SetTrigger(_triggerName);
    }
}