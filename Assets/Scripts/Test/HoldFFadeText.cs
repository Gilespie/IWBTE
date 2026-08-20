using UnityEngine;

public class HoldFFadeText : FadeTextParticle
{
    protected override void Update()
    {
        if (!_isPlayerInside || _isUsed || _collider.enabled == false) return;

        if (Input.GetKey(KeyCode.F))
        {
            _isUsed = true;
            ChangeState();
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}