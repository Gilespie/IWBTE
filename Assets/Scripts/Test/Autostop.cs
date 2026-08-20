using UnityEngine;

public class Autostop : MonoBehaviour
{
    //[SerializeField] bool _isClosing = false;
    [SerializeField] Animator _animator;
    [SerializeField] float _passDelay = 30f;
    [SerializeField] float _closeDelay = 10f;
    //private float _timer = 0f;

    /*void Start()
    {
        _animator.SetBool("isClosing", false);
    }*/

    /*private void Update()
    {
        _timer += Time.deltaTime;

        if (!_isClosing && _timer >= _passDelay)
        {
            ChangeState(true);
            _timer = 0f;
        }
        else if (_isClosing && _timer >= _closeDelay)
        {
            ChangeState(false);
            _timer = 0f;
        }
    }*/

    public void ChangeStateImmediatly()
    { 
        _animator.SetBool("isClosing", true);
        //_isClosing = true;
        //_timer = 0f;
    }

    /*void ChangeState(bool value)
    {
        _isClosing = value;
        _animator.SetBool("isClosing", _isClosing);
    }*/
}