using UnityEngine;


public class FallingCeiling : MonoBehaviour
{
    [SerializeField] float _damage = 600f;
    [SerializeField] string _triggerName = "onFall";
    Animator _animator;

    void OnEnable()
    {
        EventManager.Subscribe(EventType.OnTimeOut,SetTrigger);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnTimeOut, SetTrigger);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.InstantKill();
        }
    }

    public void SetTrigger(params object[] parameters)
    {
        _animator.SetTrigger(_triggerName);
    }
}