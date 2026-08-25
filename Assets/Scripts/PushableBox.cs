using UnityEngine;

public class PushableBox : MonoBehaviour, IPushable
{
    /*public bool CanPush(PushingRaycast interactor)
    {
        return true;
    }*/

    private Character _pushingCharacter;

   /* public void Pushing(PushingRaycast interactor)
    {
        Character pushController = interactor.GetComponentInParent<Character>();

        Debug.Log(pushController.name);

        if (pushController != null)
        {
            pushController.StartPush(this);
        }
    }*/

    public void Pushing(ForwardRaycast interactor)
    {
        if (_pushingCharacter != null) return;

        Character character = interactor.GetComponentInParent<Character>();
        if (character == null) return;

        _pushingCharacter = character;
        character.StartPush(this);
    }

    public void StopPushing()
    {
        if (_pushingCharacter == null) return;

        _pushingCharacter.StopPush();
        _pushingCharacter = null;
    }
}


/* [SerializeField] Player _player;
     [SerializeField] float _standDrag = 5f;
     [SerializeField] float _moveDrag = 0f;
     [SerializeField] float _standMass = 50f;
     [SerializeField] float _moveMass = 0.5f;
     Rigidbody _rb;

     void Awake()
     {
         _rb = GetComponent<Rigidbody>();
         _rb.mass = _standMass;

         //_rb.drag = _standDrag;
         //_rb.mass = _standMass;
     }

     void Start()
     {
         _player = FindObjectOfType<Player>();
     }

     public void Pushing()
     {
         if (_player.CanMove)
         {
             StartMoving();
         }
         else
         {
             StopMoving();
         }
     }

     public void StartMoving()
     {
         FixedJoint joint = gameObject.AddComponent<FixedJoint>();
         joint.connectedBody = _player.GetComponent<Rigidbody>();
         _rb.mass = _moveMass;

         //_rb.drag = _moveDrag;
     }

     public void StopMoving()
     {
         FixedJoint joint = gameObject.GetComponent<FixedJoint>();
         Destroy(joint);
         _rb.mass = _standMass;

         //_rb.drag = _standDrag;
     }*/
