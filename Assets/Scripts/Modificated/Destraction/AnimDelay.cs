using UnityEngine;

public class AnimDelay : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _delay;

    public void ActivateAnim()
    {
        Invoke("EnableAnim", _delay);
    }

    void EnableAnim()
    {
        _animator.enabled = true;
    }
}