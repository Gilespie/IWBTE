using UnityEngine;

public class ChurchExplosion : MonoBehaviour
{
    [SerializeField] Rigidbody[] _rbs;
    [SerializeField] GameObject[] _origins;

    public void Explode()
    {
        DeactivateObjects();
        DisableKinematic();
    }

    void DeactivateObjects()
    {
        foreach(GameObject obj in _origins)
        {
            obj.SetActive(false);
        }
    }

    void DisableKinematic()
    {
        foreach (Rigidbody rb in _rbs)
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
        }
    }
}