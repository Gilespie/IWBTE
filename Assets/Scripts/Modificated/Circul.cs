using UnityEngine;

public class Circul : MonoBehaviour
{
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float _desacceleration = 1f;
    [SerializeField] float _maxSpeed = 5f;
    [SerializeField] bool _enabled = true;
    [SerializeField] Renderer[] _renderer;
    [SerializeField] AudioSource _audioSource;
    float _currentSpeed = 0f;

    void Update()
    {
        if (_enabled)
        {
            IncrementSpeed();
        }
        else
        {
            DecrementSpeed();
        }

        transform.Rotate(Vector3.right * _currentSpeed);
    }

    void IncrementSpeed()
    {
        _currentSpeed += _acceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);
    }

    void DecrementSpeed()
    {
        _currentSpeed -= _desacceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);
    }

    public void DeactivateCircul()
    {
        foreach(var renderer in _renderer)
        {
            renderer.material.SetFloat("_Alpha", 0f);
        }
        _enabled = false;
        _audioSource.Play();
    }
}