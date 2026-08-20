using UnityEngine;

public class TimelapseChanger : MonoBehaviour
{
    [SerializeField] private float _defaultTime = 1f; 
    [SerializeField] private float _slowTime = 0.2f;
    private bool _isSlow;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            _isSlow = !_isSlow;
        }

        if (_isSlow)
        {
            Time.timeScale = _slowTime;
        }
        else
        {
            Time.timeScale = _defaultTime;
        }
    }
}