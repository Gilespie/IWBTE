using UnityEngine;

public class PointFollower : MonoBehaviour
{
    [SerializeField] private Character _player;
    [SerializeField] private float _zConstant = -5f;
    [SerializeField] private float _lerpSpeed = 5f;
    [SerializeField] private Vector3 _offset;

    [Header("Bob Effect")]
    [SerializeField] private bool _isBobEnable = true;
    [SerializeField] private float _bobSpeed = 3.5f;
    [SerializeField] private float _bobIntensity = 0.05f;
    private Vector3 _bobOffset;
    private Vector3 _currentPosition;

    private void Awake()
    {
        GameManager.Instance.PointFollower = this;
    }

    void Start()
    {
        _player = GameManager.Instance.Player;
        _currentPosition = _player.transform.position + _offset + _bobOffset;
        transform.position = _currentPosition;
    }

    private void Update()
    {
        if (!_player.IsAlive) return;

        if (_isBobEnable)
        {
            BobEffect(_bobSpeed, _bobIntensity);
        }
    }

    void FixedUpdate()
    {
        _currentPosition = _player.transform.position + _offset + _bobOffset;

        _currentPosition.z = _zConstant;

        transform.position = Vector3.Lerp(transform.position, _currentPosition, _lerpSpeed * Time.fixedDeltaTime);
    }

    private void BobEffect(float speed, float intensity)
    {
        float x = Mathf.Sin(Time.time * speed) * intensity;
        float y = Mathf.Cos(Time.time * speed) * intensity;

        _bobOffset = new Vector3(x, y, 0);
    }
}