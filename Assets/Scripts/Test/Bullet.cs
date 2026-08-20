using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _delayToDelete = 3f;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _damage = 1000f;
    private Player _player;

    void Start()
    {
        Destroy(gameObject, _delayToDelete);    
    }

    void Update()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        transform.position +=  dir * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Destructable destructable))
        {
            destructable.InstantKill();
            Destroy(gameObject);
        }
    }

    public void AssingPlayer(Player player)
    {
        _player = player;
    }
}