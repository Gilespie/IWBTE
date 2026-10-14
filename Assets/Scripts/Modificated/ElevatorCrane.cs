using UnityEngine;

public class ElevatorCrane : MonoBehaviour
{
    [SerializeField] Animator _animator;
    bool _isToggled = false;

    public void ActivateBool()
    {
        _isToggled = !_isToggled;
        _animator.SetBool("isUp", _isToggled);
    }
}