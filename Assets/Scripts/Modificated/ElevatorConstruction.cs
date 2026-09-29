using System.Collections;
using UnityEngine;

public class ElevatorConstruction : MonoBehaviour
{
    [SerializeField] Animator _animatorDoors;
    [SerializeField] Animator _elevatorAnimator;
    [SerializeField] float _speed = 1f;

    public void Activate()
    {
        StartCoroutine(ActivateElevator());
    }

    private IEnumerator ActivateElevator()
    {
        CloseDoors();
        yield return new WaitForSeconds(1f);
        _elevatorAnimator.SetTrigger("Move");
        yield return null;
    }

    public void OpenDoors()
    {
        _animatorDoors.SetTrigger("Open");
    }

    public void CloseDoors()
    {
        _animatorDoors.SetTrigger("Close");
    }
}
