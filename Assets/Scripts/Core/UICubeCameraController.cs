using UnityEngine;
using UnityEngine.UI;

public class UICubeCameraController : MonoBehaviour
{
    [Header("Components")]
    public Camera cubeCamera;
    public Transform cubeRoot;
    public RawImage targetRawImage;
    public RubikCubeGenerator cubeGenerator;

    [Header("Rotation")]
    [SerializeField] private float autoRotateSpeed = 15f;

    [Header("Camera Distance (scale = 1)")]
    [Tooltip("Базовая дистанция для куба 3x3")]
    [SerializeField] private float baseDistance = 15f;

    [Tooltip("Дополнительная дистанция на каждый +1 размер")]
    [SerializeField] private float perSizeOffset = 10f;

    [Header("Camera Settings")]
    [SerializeField] private float previewFOV = 30f;

    private void Start()
    {
        SetupCamera();
        RepositionCamera();
    }

    private void Update()
    {
        if (cubeRoot == null) return;

        // Простое автоворщение
        cubeRoot.Rotate(Vector3.up, autoRotateSpeed * Time.deltaTime, Space.World);
    }

    private void SetupCamera()
    {
        if (cubeCamera == null) return;

        cubeCamera.fieldOfView = previewFOV;
        cubeCamera.nearClipPlane = 0.05f;
        cubeCamera.farClipPlane = 50f;
    }

    /// <summary>
    /// Вызывать после GenerateCube()
    /// </summary>
    public void RepositionCamera()
    {
        if (cubeCamera == null || cubeRoot == null) return;

        int size = cubeGenerator != null ? cubeGenerator.cubeSize : 3;

        float distance = baseDistance + (size - 3) * perSizeOffset;

        cubeCamera.transform.position =
            cubeRoot.position + new Vector3(0f, 0f, -distance);

        cubeCamera.transform.LookAt(cubeRoot.position);
    }
}



