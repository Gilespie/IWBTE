using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private string _boolName = "isOpened";
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void ActivateDoor(bool state)
    {
        _animator.SetBool(_boolName, state);
    }
}