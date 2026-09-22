using UnityEngine;

public class PushableBox : MonoBehaviour, IGrabbable, IPushable
{
    [SerializeField] Transform _leftHandPoint;
    [SerializeField] Transform _rightHandPoint;

    public Transform LeftHandPoint => _leftHandPoint;
    public Transform RightHandPoint => _rightHandPoint;

    private Character _pushingCharacter;
    Rigidbody _rb;
    public Rigidbody Rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());

    public void Pushing(ForwardRaycast interactor)
    {
        if (_pushingCharacter != null) return;

        Character character = interactor.GetComponentInParent<Character>();
        if (character == null) return;

        _pushingCharacter = character;
        character.StartPush(this);
    }

    public void StopPushing()
    {
        if (_pushingCharacter == null) return;

        _pushingCharacter.StopPush();
        _pushingCharacter = null;
    }
}