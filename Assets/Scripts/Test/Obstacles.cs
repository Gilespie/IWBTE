using UnityEngine;

public class Obstacles : MonoBehaviour
{
    [SerializeField] private float _damage = 1000f;

    private void OnCollisionEnter(Collision collision)
    {
        Destructable dest = collision.gameObject.GetComponent<Destructable>();
        Debug.Log(collision.gameObject.name);
        if (dest != null)
        {
            dest.InstantKill();
        }
    }
}