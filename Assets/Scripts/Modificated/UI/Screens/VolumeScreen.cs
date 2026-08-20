using UnityEngine;
using UnityEngine.UI;

public class VolumeScreen : ScreenBase
{
    [Header("Buttons")]
    [SerializeField] Button _backBTN;

    void Awake()
    {
        _backBTN.onClick.AddListener(() => ScreenManager.Instance.DeactivateScreen());
    }
}