using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer _mesh;
    [SerializeField] VisualEffect _effect;
    [SerializeField] float _refreshRate = 3;

    private void Start()
    {
        StartCoroutine(UpdateVFXGraph());
    }

    IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Mesh m = new Mesh();
            _mesh.BakeMesh(m);
            _effect.SetMesh("SkinnedMesh", m);

            yield return new WaitForSeconds(_refreshRate);
        }
    }
}
