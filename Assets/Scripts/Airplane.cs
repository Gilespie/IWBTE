using UnityEngine;

public class Airplane : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Lights[] _lights = new Lights[4];

    [SerializeField] bool _isLanding = false;
    [SerializeField] bool _isCrashing = false;
    [SerializeField] bool _isCrashing2 = false;

    private string _boolCrashName = "isCrashing";
    private string _boolCrash2Name = "isCrashing2";
    private string _boolLandName = "isLanding";

    public void PlayAnim()
    {
        if (_isLanding)
        {
            _animator.SetTrigger(_boolLandName);
        }
        else if (_isCrashing)
        {
            _animator.SetTrigger(_boolCrashName);
        }
        else if(_isCrashing2)
        {
            _animator.SetTrigger(_boolCrash2Name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Character player))
        {
            player.InstantKill();
        }
    }
}