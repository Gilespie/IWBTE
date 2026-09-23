using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    [SerializeField] Animator _animator;
    int count = 1;

    public void ActivateGlitchLights()
    {
        _animator.SetInteger("Value", count);
    }
}