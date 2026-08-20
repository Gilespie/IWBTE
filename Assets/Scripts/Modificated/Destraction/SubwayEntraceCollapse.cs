using UnityEngine;

public class SubwayEntraceCollapse : MonoBehaviour
{
    [SerializeField] Rigidbody[] _rbs;
    [SerializeField] MeshRenderer[] _meshes;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Airplane>(out _))
        {
            ActivateRBs();
        }
    }

    void ActivateRBs()
    {
        foreach (var mesh in _meshes)
        {
            mesh.enabled = false;
        }

        foreach (Rigidbody rb in _rbs)
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
        }
    }
}
