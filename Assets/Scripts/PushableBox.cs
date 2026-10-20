using UnityEngine;

[System.Serializable]
public class PushFace
{
    public Transform LeftHand;
    public Transform RightHand;
}

public class PushableBox : MonoBehaviour, IGrabbable, IPushable
{
    /*[SerializeField] Transform _leftHandPoint;
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
    }*/

    // 0: +Z, 1: -Z, 2: +X, 3: -X (в локальных осях коробки)
    [SerializeField] PushFace[] _faces = new PushFace[4];
    [SerializeField] float _maxGrabAngle = 40f;

    int _currentFace;
    Character _pushingCharacter;
    Rigidbody _rb;

    public Rigidbody Rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());
    public Vector3 PushDirection { get; private set; }   // куда смотрит персонаж (к коробке)

    public Transform LeftHandPoint => _faces[_currentFace].LeftHand;
    public Transform RightHandPoint => _faces[_currentFace].RightHand;

    Vector3 GetFaceNormal(int face) => face switch
    {
        0 => transform.forward,
        1 => -transform.forward,
        2 => transform.right,
        _ => -transform.right
    };

    public bool Pushing(ForwardRaycast interactor)
    {
        if (_pushingCharacter != null) return false;

        Character character = interactor.GetComponentInParent<Character>();
        if (character == null) return false;

        // с какой стороны стоит персонаж
        Vector3 local = transform.InverseTransformPoint(character.transform.position);
        int face = Mathf.Abs(local.z) > Mathf.Abs(local.x)
            ? (local.z > 0 ? 0 : 1)
            : (local.x > 0 ? 2 : 3);

        Vector3 outward = GetFaceNormal(face);

        // смотрит ли персонаж на эту грань
        float angle = Vector3.Angle(character.MeshTransform.forward, -outward);
        if (angle > _maxGrabAngle) return false;

        _currentFace = face;
        PushDirection = -outward;
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