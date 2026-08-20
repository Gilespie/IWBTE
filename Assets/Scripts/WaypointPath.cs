using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    private int _nextWaypointIndex = 0;

    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        _nextWaypointIndex = currentWaypointIndex + 1;

        if (_nextWaypointIndex == transform.childCount)
        {
            _nextWaypointIndex = 0;
        }

        return _nextWaypointIndex;
    }
}