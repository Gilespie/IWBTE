using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator _animator;
    int _pushLayer = 1;

    public void SetBool(int param, bool value)
    {
        _animator.SetBool(param, value);
    }

    public void SetFloat(int param, float value)
    {
        _animator.SetFloat(param, value);
    }

    public void SetTrigger(int param)
    {
        _animator.SetTrigger(param);
    }

    public void SetLayerWeight(float weight)
    {
        _animator.SetLayerWeight(_pushLayer, weight);
    }
}