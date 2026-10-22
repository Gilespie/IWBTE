using UnityEngine;

public class CranePlate : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] LayerMask _slideMask;

    private void Awake()
    {
        EventManager.Subscribe(EventType.OnExplosion, OnExplosion);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventType.OnExplosion, OnExplosion);
    }

    void OnExplosion(params object[] args)
    {
        _rb.isKinematic = false;
        gameObject.layer = (int)Mathf.Log(_slideMask.value, 2);
    }
}
