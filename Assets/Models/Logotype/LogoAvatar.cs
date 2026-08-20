using UnityEngine;

public class LogoAvatar : MonoBehaviour
{
    [SerializeField] string _levelName = "";

    public void SceneTransition()
    {
        EventManager.Trigger(EventType.OnSceneTransition, _levelName);
    }
}