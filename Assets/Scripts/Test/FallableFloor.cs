using UnityEngine;

public class FallableFloor : MonoBehaviour
{
    [SerializeField] Rigidbody[] _rbs;

    public void DestroyFloor()
    {
        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].isKinematic = false;
        }
    }
}