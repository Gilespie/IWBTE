using System;
using UnityEngine;

public class SubteDeadZone : MonoBehaviour
{
    public event Action OnEntered = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable)) return;

        OnEntered?.Invoke();
        damageable.InstantKill();
    }
}