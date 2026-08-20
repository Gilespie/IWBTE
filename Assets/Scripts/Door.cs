using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string _boolName = "isOpened";
    private Animator _animator;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void ChangeState(bool state)
    {
        _animator.SetBool(_boolName, state);
    }
}