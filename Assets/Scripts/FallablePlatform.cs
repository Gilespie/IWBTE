using System.Collections;
using UnityEngine;

public class FallablePlatform : MonoBehaviour
{
    //[SerializeField] private Player _player;
    [SerializeField] private float _delay = 0.05f;
    [SerializeField] private float _delayToDelete = 30f;
    private Rigidbody _rigidbody;
    private Coroutine _currentRoutine = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_currentRoutine != null) return;

        if(collision.gameObject.TryGetComponent(out Player player))
        {
           _currentRoutine = StartCoroutine(ActivateKinemticRoutine());
        }
    }

    private IEnumerator ActivateKinemticRoutine()
    {
        yield return new WaitForSeconds(_delay);
        _rigidbody.isKinematic = false;
        Destroy(gameObject, _delayToDelete);
        yield return null;
    }
}