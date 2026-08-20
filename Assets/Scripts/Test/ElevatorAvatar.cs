using UnityEngine;

public class ElevatorAvatar : MonoBehaviour
{
    public void OnFall()
    {
        EventManager.Trigger(EventType.OnLiftFalled);
    }
}