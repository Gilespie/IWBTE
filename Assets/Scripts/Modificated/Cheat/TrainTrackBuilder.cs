using UnityEngine;

[ExecuteAlways]
public class TrainTrackBuilder : MonoBehaviour
{
    [SerializeField] TrainWaypoint[] _waypoints;
    int _currentWaypointIndex = 0;

    void Start()
    {
        _waypoints = transform.GetComponentsInChildren<TrainWaypoint>();
    }

    public Transform GetCurrentWaypoint()
    {
        return _waypoints[_currentWaypointIndex].transform;
    }

    public int NextWaypoint()
    {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        return _currentWaypointIndex;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < _waypoints.Length - 1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_waypoints[i].transform.position, _waypoints[i + 1].transform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_waypoints[i].transform.position, 0.5f);
        }  
    }
}