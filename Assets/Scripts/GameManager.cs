using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private Character _character;

    public Character Player
    {
        get { return _character; }
        set { _character = value; }
    }

    private Vector3 _actualCheckpoint;

    public Vector3 ActualCheckpoint
    {
        get { return _actualCheckpoint; }
        set { _actualCheckpoint = value; }
    }

    /*private Transform[] _aiDogNodes;

    public Transform[] AIDogNodes
    {
        get { return _aiDogNodes; }
        set { _aiDogNodes = value; }
    }

    private NavMeshSurface _surface;

    public NavMeshSurface Surface
    { 
        get { return _surface; }
        set { _surface = value; }
    }*/

/*    private CameraFollower _cameraFollower;

    public CameraFollower Camera
    {
        get { return _cameraFollower; }
        set { _cameraFollower = value; }
    }*/

    private CameraPointFollow _cameraPointFollow;

    public CameraPointFollow CameraPointFollow
    {
        get { return _cameraPointFollow; }
        set { _cameraPointFollow = value; }
    }

    private PointFollower _pointFollower;
    public PointFollower PointFollower
    {
        get { return _pointFollower; }
        set { _pointFollower = value; }
    }
}