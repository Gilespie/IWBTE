using UnityEngine;

public class GlassPanels : MonoBehaviour
{
    [SerializeField] GameObject[] _glassParts;
    [SerializeField] Rigidbody _rb;
    [SerializeField] MeshRenderer _mesh;
    [SerializeField] Collider _col;
    [SerializeField] AudioSource _audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null)
        {
            _rb.isKinematic = true;
            _mesh.enabled = false;
            _col.enabled = false;
            _audioSource.Play();

            foreach (var part in _glassParts)
            {
                part.SetActive(true);

                Rigidbody[] rbs = part.GetComponentsInChildren<Rigidbody>();

                foreach (var rb in rbs)
                {
                    rb.AddExplosionForce(50f, transform.position, 5f, 3f, ForceMode.Impulse);
                }
            }
        }
    }
}