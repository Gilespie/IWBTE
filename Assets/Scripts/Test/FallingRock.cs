using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField] float _damage = 600f;
    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    public void PlayAnim()
    {
        _animator.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.InstantKill();
        }
    }
}