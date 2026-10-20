using UnityEngine;

[System.Serializable]
public class SaveGameData
{
    public float x; 
    public float y; 
    public float z;
    public bool IsAlive;
    public string СurrentSceneName;
    public bool IsNewGame;
    public bool IntroPlayed;

    [Header("Camera")]
    public bool HasCameraState;
    public int CameraMoveMode;
    public float CameraLerpRate;
    public float CameraMoveTowardsRate;
    public Vector3 CameraOffset;
}