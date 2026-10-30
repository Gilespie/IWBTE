using UnityEngine;

public class WoodPlank : MonoBehaviour
{
    [Header("Original Mesh")]
    [SerializeField] Collider _collider;
    [SerializeField] MeshRenderer _meshRenderer;

    [Header("Broken Mesh")]
    [SerializeField] GameObject[] _brokenMeshs;
    [SerializeField] bool _usePhysics = true;
    [SerializeField] Rigidbody[] _rbs;
    [SerializeField] GameObject _vfx;

    void Awake()
    {
        _vfx.SetActive(false);

        if (_usePhysics)
        {
            ChangePlankSkin();
            SetKinematic(true);
            return;
        }

        ShowBrokenParts(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable dmg)) return;
        
        if(_usePhysics)
        {
            SetKinematic(false);
            _vfx.SetActive(true);
            return;
        }

        ChangePlankSkin();
        
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

    void SetKinematic(bool value)
    {
        foreach (var rb in _rbs)
        {
            rb.isKinematic = value;
        }
    }
}