using System.Collections;
using UnityEngine;

public class Buildboard : MonoBehaviour
{
    [SerializeField] GameObject[] _planks;
    [SerializeField] float _rotateDuration = 0.5f;
    [SerializeField] float _delayBetween = 1f;
    [SerializeField] float _delayBetweenWaves = 5f;
    Coroutine _coroutine;

    private void Start()
    {
        _coroutine = StartCoroutine(WaveRoutine());
    }
    
    IEnumerator WaveRoutine()
    {
        while (true)
        {
            for (int i = 0; i < _planks.Length; i++)
            {
                StartCoroutine(RotatePlank(_planks[i].transform));
                yield return new WaitForSeconds(_delayBetween);
            }

            yield return new WaitForSeconds(_delayBetweenWaves);
        }
    }

    IEnumerator RotatePlank(Transform plank)
    {
        float time = 0;
        Quaternion startRot = plank.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, 120, 0);

        while (time < _rotateDuration)
        {
            time += Time.deltaTime;
            float t = time / _rotateDuration;

            plank.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        plank.rotation = targetRot;
    }
}
