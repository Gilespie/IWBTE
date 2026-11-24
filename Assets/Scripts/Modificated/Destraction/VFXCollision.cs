using UnityEngine;

public class VFXCollision : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] ParticleSystem _dustPrefab;
    [SerializeField] float _effectLifetime = 5f;

    [Header("Impact")]
    [SerializeField] float _minImpactSpeed = 5f;
    [SerializeField] LayerMask _groundLayer;  
    [SerializeField] float _cooldown = 0.2f;
    [SerializeField] bool _scaleByImpact = true;

    float _lastEffectTime = -999f;

    void OnCollisionEnter(Collision collision)
    {
        if ((_groundLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < _minImpactSpeed) return;

        if (Time.time - _lastEffectTime < _cooldown) return;
        _lastEffectTime = Time.time;

        ContactPoint contact = collision.GetContact(0);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        ParticleSystem dust = Instantiate(_dustPrefab, contact.point, rotation);

        /*
        if (_scaleByImpact)
        {
            float scale = Mathf.Clamp(impactSpeed / _minImpactSpeed, 1f, 3f);
            dust.transform.localScale *= scale;
        }*/

        dust.Play();
        Destroy(dust.gameObject, _effectLifetime);
    }
}
