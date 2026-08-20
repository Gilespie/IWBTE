using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private UnityEvent _onActivated;
    [SerializeField] private UnityEvent _onDeactivated;
    [SerializeField] private string _weightName = "isWeight";
    [SerializeField] private float _weightRequired = 30f;
    private List<Rigidbody> _rigidMass = new List<Rigidbody>();
    private float _currentWeight = 0f;
    private Animator _animator;
    private bool _isActivated = false;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            Debug.Log(rb.gameObject.name);
            _rigidMass.Add(rb);
            RecalculateWeight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            _rigidMass.Remove(rb);
            RecalculateWeight();
        }
    }

    private void RecalculateWeight()
    {
        _currentWeight = 0f;

        foreach (Rigidbody rb in _rigidMass)
        {
            if(rb != null) _currentWeight += rb.mass;
        }

        _animator.SetFloat(_weightName, _currentWeight);

        if (_currentWeight >= _weightRequired && !_isActivated)
        {
            _onActivated?.Invoke();
            _isActivated = true;
        }
        else if(_currentWeight < _weightRequired && _isActivated)
        {
            _onDeactivated?.Invoke();
            _isActivated = false;
        }
    }
}