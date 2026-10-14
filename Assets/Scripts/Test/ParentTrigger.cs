using UnityEngine;

public class ParentTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out Character character)) return;

        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Character character)) return;

        other.transform.SetParent(null);
    }
}