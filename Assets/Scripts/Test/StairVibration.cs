using UnityEngine;

public class StairVibration : MonoBehaviour
{
    Animator _animator;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void ActivateAnimation()
    {
        _animator.enabled = true;
    }
}