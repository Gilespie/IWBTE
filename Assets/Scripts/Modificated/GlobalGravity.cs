using UnityEngine;

public class GlobalGravity : MonoBehaviour
{
    [SerializeField] float gravity = -20f;

    void Awake()
    {
        Physics.gravity = new(0, gravity, 0);
    }
}