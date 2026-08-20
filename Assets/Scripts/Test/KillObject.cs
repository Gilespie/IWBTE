using UnityEngine;

public class KillObject : MonoBehaviour
{
    [SerializeField] private float speedThreshold = 2f; // ����� ��������
    [SerializeField] private float damageAmount = 20f; // ����

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        if (currentSpeed > speedThreshold)
        {
            Destructable destruct = collision.gameObject.GetComponent<Destructable>();

            if (destruct != null)
            {
                destruct.InstantKill();
                Debug.Log($"������ ������ ������ �� ��������� {currentSpeed}, �������� {damageAmount} �����!");
            }

        }

    }
}