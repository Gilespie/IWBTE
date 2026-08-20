using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private Character _parent;

    void Start()
    {
        _parent = GetComponentInParent<Character>();    
    }

    public void Pressing()
    {
        _parent.Pressing();
    }

    public void PlayJump()
    {
        _parent.PlayJump();
    }

    public void ActivateKinematic()
    {
        _parent.ActivateRBKinematic();
    }

    public void DeactiveKinematic()
    {
        _parent.DeactivateRBKinematic();
    }
}