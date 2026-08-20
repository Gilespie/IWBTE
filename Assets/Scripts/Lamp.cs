using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    
    public void TakeImpulse(float magnitude)
    {
        if(_rb != null) _rb.AddForce(_rb.position *  magnitude, ForceMode.Impulse);
    }

    public void ActivateLamp(bool active)
    {
        if (active)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }  
    }
}