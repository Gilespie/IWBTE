using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class WaterZone : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private float _dragInWater = 10f;
    [SerializeField] private float _dragInAir = 1f;
    [SerializeField] private float slowDuration = 1.0f;
    [SerializeField] private float slowFallPower = 8f;
    [SerializeField] private Volume _volume;
    [SerializeField] private Collider _collider;

    [Header("Splash Effect")]
    [SerializeField] private GameObject _splashPrefab;
    public float BoundY => _collider.bounds.max.y;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        SpawnSplashes(other.transform.position);

        if (other.TryGetComponent(out AirManager air))
        {
            air.SetWaterZone(this);
        }

        if(other.TryGetComponent(out CameraPointFollow camera))
        {
            camera.SetWaterZone(this);
            camera.SetUnderwaterVolume(_volume);
            camera.EnterWater();
        }

        if (other.TryGetComponent(out Player player))
        {
            player.SetInWaterZone(true);
            player.SetWaterZone(this);

            if (player.TryGetComponent(out Rigidbody playerRB))
            {
                StartCoroutine(SlowFallInWater(playerRB));
            }
        }
        
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.linearDamping = _dragInWater;
        }
        
        if(other.TryGetComponent(out Character character))
        {
            character.SetWaterZone(this);

            if (character.TryGetComponent(out Rigidbody characterRB))
            {
                characterRB.linearDamping = _dragInWater;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;

        if (other.TryGetComponent(out AirManager air))
        {
            air.SetWaterZone(null);
        }

        if (other.TryGetComponent(out CameraPointFollow camera))
        {
            //camera.SetWaterZone(this);
            //camera.SetVolume(_volume);
            camera.ExitWater();
        }

        if (other.TryGetComponent(out Player player))
        {
            player.SetInWaterZone(false);

            if (player.TryGetComponent(out Rigidbody playerRB))
            {
                playerRB.linearDamping = _dragInAir;
            }
        }
        
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.linearDamping = _dragInAir;
        }
        
        if(other.TryGetComponent(out Character character))
        {
            character.SetWaterZone(null);

            if (character.TryGetComponent(out Rigidbody characterRB))
            {
                characterRB.linearDamping = _dragInAir;
            }
        }
    }

    private void SpawnSplashes(Vector3 pos)
    {
        if (_splashPrefab != null)
        {
            GameObject splash = Instantiate(_splashPrefab, pos, _splashPrefab.transform.rotation);
            splash.GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator SlowFallInWater(Rigidbody rb)
    {
        float timer = 0f;
        Vector3 initialVelocity = rb.linearVelocity;

        while (timer < slowDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slowDuration;

            float newY = Mathf.Lerp(initialVelocity.y, 0f, t * slowFallPower * Time.deltaTime);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, newY, rb.linearVelocity.z);

            yield return null;
        }

        Vector3 finalVelocity = rb.linearVelocity;
        finalVelocity.y = 0f;
        rb.linearVelocity = finalVelocity;
    }
}