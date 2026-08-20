using UnityEngine;

public class Tesla : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] int clip;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable character = other.GetComponent<IDamageable>();

        if (character != null)
        {
            character.InstantKill();
        }
    }

    public void PlayAnim()
    {
        animator.SetInteger("ClipIndex", clip);
    }
}