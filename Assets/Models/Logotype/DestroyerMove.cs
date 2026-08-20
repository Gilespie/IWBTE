using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerMove : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    void Update()
    {
        transform.Translate(Vector3.right * m_Speed * Time.deltaTime);
    }
}
