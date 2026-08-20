using UnityEngine;

public class NodeAssigner : MonoBehaviour
{
    private void Awake()
    {
        Transform[] nodes = GetComponentsInChildren<Transform>();
        //GameManager.Instance.AIDogNodes = nodes;
    }
}