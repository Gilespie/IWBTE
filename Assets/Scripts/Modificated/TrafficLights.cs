using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    [SerializeField] Animator[] _animators;

    public void ActivateGlitchLights()
    {
        int count = 1;
        foreach (var anim in _animators)
        {
            anim.SetInteger("Value", count);
            count++;
        }
    }
}