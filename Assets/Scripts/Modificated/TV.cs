using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;

    public void PlayVideo(VideoClip clip, bool isLooping)
    {
        _videoPlayer.clip = clip;
        _videoPlayer.isLooping = isLooping;
        _videoPlayer.Play();
    }
}