using UnityEngine;

public class SteelFanceDamage : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshNormal;
    [SerializeField] GameObject _meshDamaged;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponentInParent<BusMovement>())
        {
            _meshNormal.enabled = false;
            _meshDamaged.SetActive(true);
        }

        
        /*if(collision.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.InstantKill();
        }*/
    }
}