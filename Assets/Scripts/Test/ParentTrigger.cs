using UnityEngine;

public class ParentTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out Character character)) return;

        character.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Character character)) return;

        character.transform.SetParent(null);
    }
}