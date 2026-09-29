using UnityEngine;

public class PushableBox : MonoBehaviour, IGrabbable, IPushable
{
    [SerializeField] Transform _leftHandPoint;
    [SerializeField] Transform _rightHandPoint;
    [SerializeField] private float _minGrabAngle = 160f;
    [SerializeField] private float _maxGrabAngle = 210f;
    float _currentangle;
    public Transform LeftHandPoint => _leftHandPoint;
    public Transform RightHandPoint => _rightHandPoint;

    private Character _pushingCharacter;
    Rigidbody _rb;
    public Rigidbody Rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());

    public bool Pushing(ForwardRaycast interactor)
    {
        if (_pushingCharacter != null) return false; 

        Character character = interactor.GetComponentInParent<Character>();
        if (character == null) return false;

        Transform meshTransform = character.MeshTransform;

        float signedAngle = Vector3.SignedAngle(meshTransform.forward, -transform.forward, Vector3.up);
        float angle360 = (signedAngle + 360f) % 360f;
        Debug.Log(angle360);
        if (angle360 < _minGrabAngle || angle360 > _maxGrabAngle) return false;

        _pushingCharacter = character;
        character.StartPush(this);

        return true;
    }

    public void StopPushing()
    {
        if (_pushingCharacter == null) return;

        _pushingCharacter.StopPush();
        _pushingCharacter = null;
    }
}