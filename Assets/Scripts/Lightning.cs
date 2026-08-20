using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 10f;
    [SerializeField] private float noiseScale = 1f;
    [SerializeField] private float horizontalAmplitude = 1f;
    [SerializeField] private float lifetime = 3f;

    [Header("Branching")]
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private float branchChance = 0.3f; // Вероятность ветвления
    private bool hasBranched = false;

    private float _elapsedTime = 0f;
    private Vector3 _startPos;
    private float _seedX;
    private float _seedZ;

    private void Start()
    {
        _startPos = transform.position;
        _seedX = Random.Range(0f, 100f);
        _seedZ = Random.Range(0f, 100f);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        float y = _startPos.y - fallSpeed * _elapsedTime;
        float x = _startPos.x + (Mathf.PerlinNoise(_seedX, _elapsedTime * noiseScale) - 0.5f) * horizontalAmplitude * 2f;
        float z = _startPos.z + (Mathf.PerlinNoise(_seedZ, _elapsedTime * noiseScale) - 0.5f) * horizontalAmplitude * 2f;

        transform.position = new Vector3(x, y, z);

        // Создание ветки на середине пути
        if (!hasBranched && _elapsedTime > lifetime / 2f)
        {
            hasBranched = true;

            if (branchPrefab != null && Random.value < branchChance)
            {
                Vector3 branchDir = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(-1f, 1f)).normalized;
                GameObject branch = Instantiate(branchPrefab, transform.position, Quaternion.identity);
                Lightning branchScript = branch.GetComponent<Lightning>();

                if (branchScript != null)
                {
                    // Слегка отличающиеся параметры
                    branchScript.fallSpeed = fallSpeed * Random.Range(0.8f, 1.2f);
                    branchScript.noiseScale = noiseScale * Random.Range(0.8f, 1.2f);
                    branchScript.horizontalAmplitude = horizontalAmplitude * 0.5f;
                    branchScript.lifetime = lifetime * 0.5f;
                }
            }
        }

        if (_elapsedTime > lifetime)
        {
            Destroy(gameObject);
        }
    }
}