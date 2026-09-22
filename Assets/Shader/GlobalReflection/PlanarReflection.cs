using UnityEngine;

public class PlanarReflection : MonoBehaviour
{
    [SerializeField] Camera _reflectCamera;
    [SerializeField] RenderTexture _reflectTexture;
    [SerializeField] int _reflectResolution;
    Vector2 _currentResolution;

    private void LateUpdate()
    {
        float planeY = transform.position.y;
        Vector3 mainPos = Camera.main.transform.position;

        _reflectCamera.fieldOfView = Camera.main.fieldOfView;

        _reflectCamera.transform.position = new Vector3(
            mainPos.x,
            2f * planeY - mainPos.y,
            mainPos.z);

        Vector3 mainEuler = Camera.main.transform.eulerAngles;
        _reflectCamera.transform.rotation = Quaternion.Euler(-mainEuler.x, mainEuler.y, 0f);

        _currentResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        _reflectTexture.Release();
        _reflectTexture.width = Mathf.RoundToInt(_currentResolution.x) * _reflectResolution / Mathf.RoundToInt(_currentResolution.y);
        _reflectTexture.height = _reflectResolution;
    }
}