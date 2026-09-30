using UnityEngine;

public class WoodPlank : MonoBehaviour
{
    [Header("Original Mesh")]
    [SerializeField] Collider _collider;
    [SerializeField] MeshRenderer _meshRenderer;

    [Header("Broken Mesh")]
    [SerializeField] GameObject[] _brokenMeshs;

    void Awake()
    {
        ShowBrokenParts(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable dmg))
        {
            ChangePlankSkin();
        }
    }

    void ChangePlankSkin()
    {
        DisableComponents();
        ShowBrokenParts(true);
    }

    void DisableComponents()
    {
        _collider.enabled = false;
        _meshRenderer.enabled = false;
    }

    void ShowBrokenParts(bool value)
    {
        foreach (var brokenMesh in _brokenMeshs)
        {
            brokenMesh.SetActive(value);
        }
    }
}