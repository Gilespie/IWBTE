using UnityEngine;
using UnityEngine.Video;

public class TVHouse : MonoBehaviour
{
    [SerializeField] VideoClip _clipGlitch;
    [SerializeField] TV[] _tvs;

    public void PlayGlitch()
    {
        foreach (var tv in _tvs)
        {
            tv.PlayVideo(_clipGlitch, false);
        }
    }
}
