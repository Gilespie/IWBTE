using UnityEngine;

public interface IControllable
{
    Transform HandAnchor { get; }
    void SetInputValue(float value);
    float GetAxisValue(Vector2 rawInput);
    void OnRelease();
}