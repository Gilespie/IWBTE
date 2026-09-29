using System.Collections;
using UnityEngine;

public class BrickFalls : MonoBehaviour
{
    [SerializeField] Rigidbody[] _bricks;
    [SerializeField] float _resetTime = 5f;

    void Start()
    {
        ActivateKinematic(true);
    }

    void ActivateKinematic(bool value)
    {
        foreach (var brick in _bricks)
        {
            brick.isKinematic = value;
        }  
    }

    public void ActivateBricks()
    {
        ActivateKinematic(false);
        StartCoroutine(ResetKinematic());
    }

    IEnumerator ResetKinematic()
    {
        yield return new WaitForSeconds(_resetTime);
        ActivateKinematic(true);
    }
}