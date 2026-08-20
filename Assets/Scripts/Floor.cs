using UnityEngine;

public enum TypeOfObject
{
    Player,
    Other
}

public class Floor : MonoBehaviour
{
    [SerializeField] private TypeOfObject _type = TypeOfObject.Player;
    [SerializeField] private GameObject _hitPrefab;
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_type == TypeOfObject.Player)
        {
            if (collision.gameObject.TryGetComponent<Player>(out Player player))
            {
                rb.isKinematic = false;
            }
        }
        else if (_type == TypeOfObject.Other)
        {
            if(collision.gameObject.GetComponent<InteractObject>())
            {
                rb.isKinematic = false;
                Instantiate(_hitPrefab, collision.contacts[0].point, Quaternion.identity);
                collision.gameObject.GetComponent<InteractObject>().PlaySound();
            }
        }
        
    }
}