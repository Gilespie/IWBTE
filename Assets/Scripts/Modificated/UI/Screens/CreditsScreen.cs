using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : ScreenBase
{
    [Header("Buttons")]
    [SerializeField] Button _backBTN;

    void Awake()
    {
        _backBTN.onClick.AddListener(() => ScreenManager.Instance.DeactivateScreen());
    }
}