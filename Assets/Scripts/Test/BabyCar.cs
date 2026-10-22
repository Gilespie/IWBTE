using UnityEngine;

public class BabyCar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<Rocket>(out _)) return;

        gameObject.SetActive(false);
    }
}