using UnityEngine;

public interface IGrabbable
{
    Transform LeftHandPoint { get; }
    Transform RightHandPoint { get; }
}