using System.Collections;
using UnityEngine;
public enum TargetMode
{
    Random,
    Sequential
}

public class RocketSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject _rocketPrefab;
    [SerializeField] Transform _spawnPosition;
    [SerializeField] Transform[] _targets;

    [SerializeField] float _timeToSpawn = 3f;
    [SerializeField] bool _isLoop = true;

    [SerializeField] private TargetMode _targetMode;

    int _currentIndex = 0;
    Coroutine _attackRoutine;

    void OnEnable()
    {
        if (_isLoop)
        {
            _attackRoutine = StartCoroutine(AirAttackLoop());
        }
        else
        {
            FireSingleRocket();
        }
    }

    void OnDisable()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
        }
    }

    IEnumerator AirAttackLoop()
    {
        while (true)
        {
            FireSingleRocket();

            yield return new WaitForSeconds(_timeToSpawn);
        }
    }

    void FireSingleRocket()
    {
        Transform target = GetTarget();

        if (_targets == null)
            return;

        GameObject rocket = Instantiate(_rocketPrefab, _spawnPosition.position, Quaternion.identity);

        Rocket rocketComponent = rocket.GetComponent<Rocket>();

        rocketComponent.SetTarget(target);
    }

    Transform GetTarget()
    {
        if (_targets == null || _targets.Length == 0)
            return null;

        switch (_targetMode)
        {
            case TargetMode.Random:
                return _targets[Random.Range(0, _targets.Length)];

            case TargetMode.Sequential:
                Transform target = _targets[_currentIndex];

                _currentIndex++;

                if (_currentIndex >= _targets.Length)
                    _currentIndex = 0;

                return target;
        }

        return null;
    }
}