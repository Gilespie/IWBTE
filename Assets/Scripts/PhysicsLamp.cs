using UnityEngine;

public class PhysicsLamp : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] float _forceStrenght;
    Vector3 _force;

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.OnExplosion, AddForceImpulse);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventType.OnExplosion, AddForceImpulse);
    }

    public void AddForceImpulse(params object[] args)
    {
        _force = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _forceStrenght;
        _rigidbody.AddForce(_force, ForceMode.Impulse);
    }
}
