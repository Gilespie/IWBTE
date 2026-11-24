using UnityEngine;

public class CeramicDestroyer : MonoBehaviour
{
    [SerializeField] Rigidbody[] _ceramicRBs;
    [SerializeField] float _impulseForce = 10f;

    public void DestroyCeramics()
    {
        foreach (var rb in _ceramicRBs)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up + -Vector3.forward * _impulseForce, ForceMode.Impulse);
        }
    }
}