using UnityEngine;

public class LabelBus : MonoBehaviour
{
    [SerializeField] Material _mat;

    [Header("Settings")]
    [SerializeField] float _delay = 10f;
    [SerializeField] float _yStep = 0.333333f;

    [Header("Smooth")]
    [SerializeField] float _scrollSpeed = 0.1f;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _audioClip;

    float _timer;

    float _currentY;
    float _targetY;

    void Start()
    {
        _timer = _delay;

        Vector2 offset = _mat.GetVector("_Offset");

        _currentY = offset.y;
        _targetY = offset.y;
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _timer = _delay;

            _targetY += _yStep;

            if (_audioSource && _audioClip)
            {
                _audioSource.PlayOneShot(_audioClip);
            }
        }

        _currentY = Mathf.MoveTowards(
            _currentY,
            _targetY,
            _scrollSpeed * Time.deltaTime
        );

        Vector2 offset = _mat.GetVector("_Offset");

        offset.y = _currentY;

        _mat.SetVector("_Offset", offset);
    }

    void OnApplicationQuit()
    {
        Vector2 zero = new Vector2(0,0);
        _mat.SetVector("_Offset", zero);
    }
}