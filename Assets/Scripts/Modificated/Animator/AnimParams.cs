using UnityEngine;

public static class AnimParams
{
    public static readonly int Move = Animator.StringToHash("isMoving");
    public static readonly int Air = Animator.StringToHash("isOnAir");
    public static readonly int Crouch = Animator.StringToHash("isCrouch");
    public static readonly int Speed = Animator.StringToHash("moveSpeed");
    public static readonly int Push = Animator.StringToHash("isPushing");
    public static readonly int Slide = Animator.StringToHash("isSliding");
    public static readonly int Jump = Animator.StringToHash("onJump");
    public static readonly int Press = Animator.StringToHash("onPressed");
    public static readonly int StartSwimm = Animator.StringToHash("onStartSwimming");
    public static readonly int StopSwimm = Animator.StringToHash("onStopSwimming"); 
    public static readonly int Climb = Animator.StringToHash("Climbing");
    public static readonly int IsFalling = Animator.StringToHash("isFalling");
}