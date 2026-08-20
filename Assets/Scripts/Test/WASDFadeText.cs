using UnityEngine;

public class WASDFadeText : FadeTextParticle
{
    protected override void Update()
    {
        if (!_isPlayerInside || _isUsed || _collider.enabled == false) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
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