using System.Xml;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private GameObject[] _parts;
    [SerializeField] private MeshRenderer[] _renderers;

    void Start()
    {
        HideParts();
 
        _renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void ActivateParts()
    {
        HideMeshRenderer();

        for (int i = 0; i < _parts.Length; i++)
        {
            _parts[i].SetActive(true);
        }
    }

    private void HideMeshRenderer()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = false;
        }
    }

    private void HideParts()
    {
        for (int i = 0; i < _parts.Length; i++)
        {
            _parts[i].SetActive(false);
        }
    }
}
