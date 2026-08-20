using UnityEngine;

public class ObjectMaterial : MonoBehaviour, IStepable
{
    [SerializeField] private MaterialType _materialType;

    public MaterialType MaterialType => _materialType;
}