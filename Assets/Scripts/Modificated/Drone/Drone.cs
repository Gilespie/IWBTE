using UnityEngine;

public class Drone : MonoBehaviour, IDamageable
{
    bool _isAlive = true;
    public bool IsAlive => _isAlive;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.InstantKill();
            InstantKill();
        }
    }


    public void InstantKill(params object[] parameters)
    {
        Destroy(gameObject);
    }
}
