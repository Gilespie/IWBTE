using UnityEngine;

public class Acceleration : MonoBehaviour
{
    [SerializeField] private float _speedMultiplier = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.SetSpeedMultiplier(_speedMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.SetSpeedMultiplier(1f);
        }
    }
}