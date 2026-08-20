using UnityEngine;

public class CatwalkDestruction : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _animator.enabled = false;
    }

    [ContextMenu("DestructCatwalk")]
    public void ActivateDestruction()
    {
        _animator.enabled = true;
    }
}