using UnityEngine;

public class SPACEFadeText : FadeTextParticle
{
    protected override void Update()
    {
        if (!_isPlayerInside || _isUsed || _collider.enabled == false) return;

        if (Input.GetKeyDown(KeyCode.Space))
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