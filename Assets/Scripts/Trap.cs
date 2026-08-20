using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float _damage = 300f;
    [SerializeField] private float _impulse = 3f;
    private bool _isActive = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isActive) return;

        if(collision.gameObject.TryGetComponent(out Destructable player))
        {
            player.InstantKill();
            player.gameObject.GetComponentInParent<Rigidbody>().AddForceAtPosition(this.GetComponent<Rigidbody>().linearVelocity * _impulse, collision.contacts[0].point, ForceMode.Impulse);
            _isActive = true;
        }
    }
}