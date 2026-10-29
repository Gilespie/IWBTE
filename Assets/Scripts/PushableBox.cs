using UnityEngine;

public class PushableBox : MonoBehaviour, IGrabbable, IPushable
{
    [Header("Grab")]
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] float _maxGrabAngle = 40f;

    [Header("Hand placement")]
    [SerializeField] float _handSpacing = 0.5f;      
    [SerializeField] float _handHeight = 0f;         
    [SerializeField] float _surfaceOffset = 0.05f;   
    [SerializeField] Vector3 _leftHandRotationOffset;
    [SerializeField] Vector3 _rightHandRotationOffset;

    Character _pushingCharacter;
    Rigidbody _rb;
    Transform _leftHand;
    Transform _rightHand;

    public Rigidbody Rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());
    public Vector3 PushDirection { get; private set; }

    public Transform LeftHandPoint => _leftHand;
    public Transform RightHandPoint => _rightHand;

    void Awake()
    {
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();

        _leftHand = CreatePoint("LeftHandPoint (runtime)");
        _rightHand = CreatePoint("RightHandPoint (runtime)");
    }

    Transform CreatePoint(string pointName)
    {
        var t = new GameObject(pointName).transform;
        t.SetParent(transform, false);
        return t;
    }

    public bool Pushing(ForwardRaycast interactor)
    {
        if (_pushingCharacter != null) return false;

        Character character = interactor.GetComponentInParent<Character>();
        if (character == null) return false;

        Vector3 size = _boxCollider.size;
        Vector3 local = transform.InverseTransformPoint(character.transform.position) - _boxCollider.center;

        Vector3 nLocal = Mathf.Abs(local.z / size.z) > Mathf.Abs(local.x / size.x)
            ? new Vector3(0f, 0f, Mathf.Sign(local.z))
            : new Vector3(Mathf.Sign(local.x), 0f, 0f);

        Vector3 outward = transform.TransformDirection(nLocal).normalized;

        float angle = Vector3.Angle(character.MeshTransform.forward, -outward);
        if (angle > _maxGrabAngle) return false;

        PushDirection = -outward;
        PlaceHands(nLocal);

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

    void PlaceHands(Vector3 nLocal)
    {
        Vector3 center = _boxCollider.center;
        Vector3 size = _boxCollider.size;

        float half = Mathf.Abs(Vector3.Dot(nLocal, size)) * 0.5f;

        Vector3 surface = center + nLocal * (half + _surfaceOffset);
        surface.y = center.y + _handHeight;

        Vector3 side = Vector3.Cross(Vector3.up, -nLocal);

        Quaternion look = Quaternion.LookRotation(-nLocal, Vector3.up);

        _rightHand.localPosition = surface + side * (_handSpacing * 0.5f);
        _leftHand.localPosition = surface - side * (_handSpacing * 0.5f);

        _rightHand.localRotation = look * Quaternion.Euler(_rightHandRotationOffset);
        _leftHand.localRotation = look * Quaternion.Euler(_leftHandRotationOffset);
    }
}