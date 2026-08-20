using UnityEngine;

public class CharacterColliderResizer : MonoBehaviour 
{
    [SerializeField] CapsuleCollider _col;
    private Vector3 _defaultCenter;
    private float _defaultHeight;

    public void SetSize(float size, Vector3 center)
    {
        _col.height = size;
        _col.center = center;
    }

    public void ResetSize()
    {
        _col.height = _defaultHeight;
        _col.center = _defaultCenter;
    }

    public void InitDefault()
    {
        _defaultCenter = _col.center;
        _defaultHeight = _col.height;
    }
}