using UnityEngine;
using UnityEngine.UI;

public class KeyboardScreen : ScreenBase
{
    [SerializeField] RebindKey[] _rebindKeys;
    [SerializeField] Button _resetBTN;
    [SerializeField] Button _backBTN;

    void Awake()
    {
        _resetBTN.onClick.AddListener(ResetAllBindings);
        _backBTN.onClick.AddListener(() => ScreenManager.Instance.DeactivateScreen());
    }

    public void ResetAllBindings()
    {
        foreach (var rebindKey in _rebindKeys)
        {
            rebindKey.ResetRebind();
        }

        SaveManager.Instance.SaveGame();
    }
}