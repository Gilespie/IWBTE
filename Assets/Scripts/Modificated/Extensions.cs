using UnityEngine;

public static class Extensions
{
    public static void Jump(this Rigidbody rb, float jumpForce)
    {
        rb.AddForce(Vector3.up + rb.linearVelocity * jumpForce, ForceMode.Acceleration);
    }
}