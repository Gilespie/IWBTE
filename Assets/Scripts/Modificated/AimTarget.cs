using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public void SetPosition(Transform target)
    {
        if (target == null) return;

        transform.position = target.position; 
    }
}