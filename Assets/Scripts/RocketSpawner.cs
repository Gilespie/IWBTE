using System.Collections;
using UnityEngine;
public enum TargetMode
{
    Random,
    Sequential,
    AllTargets
}

public class RocketSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject _rocketPrefab;
    [SerializeField] Transform _spawnPosition;
    [SerializeField] Transform[] _targets;

    [SerializeField] float _timeToSpawn = 3f;
    [SerializeField] bool _isLoop = true;

    [Header("All Targets Mode")]
    [SerializeField] float _delayBetweenRockets = 0f;
    [SerializeField] private TargetMode _targetMode;

    int _currentIndex = 0;
    Coroutine _attackRoutine;

    void OnEnable()
    {
        if (_targetMode == TargetMode.AllTargets)
        {
            _attackRoutine = StartCoroutine(FireAllTargetsRoutine());
            return;
        }

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
            _attackRoutine = null;
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

    IEnumerator FireAllTargetsRoutine()
    {
        if (_targets != null)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                if (_targets[i] == null) continue;

                SpawnRocket(_targets[i]);

                if (_delayBetweenRockets > 0f)
                    yield return new WaitForSeconds(_delayBetweenRockets);
            }
        }

        _attackRoutine = null;
        gameObject.SetActive(false);
    }

    void FireSingleRocket()
    {
        if (_targets == null || _targets.Length == 0)
            return;

        SpawnRocket(GetTarget());
    }

    void SpawnRocket(Transform target)
    {
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