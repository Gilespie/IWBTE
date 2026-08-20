using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] Transform[] _points; // 0 = старт, 1 = конец
    [SerializeField] float _moveDuration = 4f; 
    [SerializeField] float _pitchBase = 0.6f;
    [SerializeField] float _pitchMultiplier = 1.5f;

    Rigidbody _rb;
    AudioSource _audioSource;

    bool _isMoving = false;
    float _elapsedTime = 0f;
    int _currentPointIndex = 0;

    Vector3 _startPos;
    Vector3 _endPos;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        
        transform.position = _points[0].position;
    }

    void FixedUpdate()
    {
        if (!_isMoving) return;

        _elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

        // Плавная кривая ускорения и замедления
        float smoothT = Mathf.SmoothStep(0f, 1f, t);
        Vector3 newPosition = Vector3.Lerp(_startPos, _endPos, smoothT);
        _rb.MovePosition(newPosition);

        // Рассчитываем "виртуальную скорость"
        float virtualSpeed = Vector3.Distance(_startPos, _endPos) * (Mathf.SmoothStep(0f, 1f, t + 0.01f) - Mathf.SmoothStep(0f, 1f, t)) / Time.fixedDeltaTime;

        // Меняем pitch
        _audioSource.pitch = _pitchBase + virtualSpeed * _pitchMultiplier;

        // Когда движение завершено
        if (t >= 1f)
        {
            transform.position = _endPos;
            _isMoving = false;
            _audioSource.pitch = _pitchBase;
            _audioSource.Stop();

            // Инвертируем цель
            _currentPointIndex = _currentPointIndex == 0 ? 1 : 0;
        }
    }

    public void StartMove()
    {
        if (_isMoving) return;

        _isMoving = true;
        _elapsedTime = 0f;
        _startPos = transform.position;

        _currentPointIndex = Vector3.Distance(transform.position, _points[0].position) < 0.1f ? 1 : 0;

        _endPos = _points[_currentPointIndex].position;

        if (_audioSource && !_audioSource.isPlaying)
            _audioSource.Play();
    }
}