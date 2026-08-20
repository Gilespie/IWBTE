using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private CursorLockMode _lockState = CursorLockMode.Locked;
    [SerializeField] private bool _isCursorVisible = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Cursor.lockState = _lockState;
        Cursor.visible = _isCursorVisible;
    }
}