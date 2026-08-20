using UnityEngine;

public class BabyCar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<DronMovement>(out _)) return;

        gameObject.SetActive(false);
    }
}