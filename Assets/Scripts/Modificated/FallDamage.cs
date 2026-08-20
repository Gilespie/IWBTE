using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [Header("Falling")]
    [SerializeField] private float _fallDamageMultiplier = 100f;
    [SerializeField] private float _fallDamageThreshold = -10f;
    private float _maxFallSpeed = 0f;
    private float _fallDamage = 0f;
    private bool _wasGrounded = false;

    public void Tick(bool isGrounded, bool isSwimming, bool isSlope, float velocityY)
    {
        if (isSwimming)
        {
            _maxFallSpeed = 0f;
            Debug.Log("swim return");
            return;
        }

        if (!isGrounded && !isSwimming)
        {
            if (!isSlope && velocityY < _maxFallSpeed) _maxFallSpeed = velocityY;
        }

        if (isGrounded && !_wasGrounded)
        {
            if (_maxFallSpeed < _fallDamageThreshold)
            {
                Debug.Log("Fall damage triggered");
                _fallDamage = Mathf.Abs(_maxFallSpeed + _fallDamageThreshold) * _fallDamageMultiplier;
                EventManager.Trigger(EventType.OnFalled);
            }

            _maxFallSpeed = 0f;
        }

        _wasGrounded = isGrounded;
    }
}