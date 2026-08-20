using UnityEngine;
using UnityEngine.VFX;

public abstract class FadeTextParticle : MonoBehaviour
{
    protected VisualEffect _fadeTextVFX;
    protected Collider _collider;
    protected bool _isPlayerInside = false;
    protected bool _isUsed = false;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _fadeTextVFX = GetComponent<VisualEffect>();
        _fadeTextVFX.Play();
    }

    protected virtual void Update()
    {
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Character player))
        {
            _isPlayerInside = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character player))
        {
            _isPlayerInside = false;
        }
    }

    protected void ChangeState()
    {
        _fadeTextVFX.SetBool("IsTriggered", true);
        _fadeTextVFX.Reinit();
        _fadeTextVFX.Play();
        Invoke(nameof(TurnOff), 10f);
        _collider.enabled = false;
    }

    protected virtual void TurnOff()
    {
        gameObject.SetActive(false);
    }
}