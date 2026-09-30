using System.Collections;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] HingeJoint _hingeJoint;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] Light _light;
    [SerializeField] float _impulseForce = 0.01f;
    [SerializeField] float _breakForce = 0.01f;
    [SerializeField] float _delayBreak = 1f;
    [SerializeField] bool _isCrashing = false;

    public void LampEvent()
    {
        TakeImpulse(_impulseForce);

        if (_isCrashing)
        {
            StartCoroutine(BreakRoutine());
        }
    }

    public void ActivateLamp(bool active)
    {
        if (active)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }  
    }

    void TakeImpulse(float magnitude)
    {
        _rb.AddForce(_rb.position *  magnitude, ForceMode.Impulse);
    }

    void BreakForceFall(float value)
    {
        _hingeJoint.breakTorque = value;
    }

    void ShoutdownLights()
    {
        _light.enabled = false;
        _renderer.material.DisableKeyword("_EMISSION");
    }

    IEnumerator BreakRoutine()
    {
        BreakForceFall(_breakForce);
        yield return new WaitForSeconds(_delayBreak);
        ShoutdownLights();
        yield return null;
    }
}