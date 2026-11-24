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

    private CameraFollow _cameraPointFollow;

    public CameraFollow CameraPointFollow
    {
        get { return _cameraPointFollow; }
        set { _cameraPointFollow = value; }
    }
}