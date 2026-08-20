using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Trigger : MonoBehaviour
{
    [SerializeField] protected bool _isOnce = false;
    [SerializeField] protected UnityEvent _actions;
    [SerializeField] protected UnityEvent _actionsStay;
    [SerializeField] protected Collider _collider;

    private void Awake()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Character>(out _)) return;
        
        _actions?.Invoke();
        
        if (_isOnce) _collider.enabled = false;
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent<Character>(out _)) return;
        
        _actionsStay?.Invoke();
    }

    [ContextMenu("Activate Trigger")]
    public void Activate()
    {
        _actions?.Invoke();
    }
}