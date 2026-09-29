using UnityEngine;
using UnityEngine.VFX;

public class SteelFenceCrash : MonoBehaviour
{
    [SerializeField] VisualEffect _sparkleVFX;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<BusMovement>())
        {
            Debug.Log("bus hit");
            _sparkleVFX.Play();
        }
    }
}
