using UnityEngine;

public class CameraPanelUI : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraOrbit cameraOrbit;

    private void Awake()
    {
        if (cameraOrbit == null)
            cameraOrbit = FindFirstObjectByType<CameraOrbit>();
    }

    


    public void SetPublicRotation()
    {
        SetMode(CameraMode.PublicRotation);
    }

    public void SetFix()
    {
        SetMode(CameraMode.Fix);
    }

    public void SetIsometric()
    {
        SetMode(CameraMode.Izometria);
    }

    private void SetMode(CameraMode mode)
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.SelectedCameraPreset = (int)mode;
        GameManager.Instance.UpdateCameraMode(mode);

        if (cameraOrbit != null)
            cameraOrbit.SetMode(mode);
    }

    public void OnChangeCameraMode()
    {
        GameManager.Instance.SelectedCameraPreset++;
        if (GameManager.Instance.SelectedCameraPreset > 2)
            GameManager.Instance.SelectedCameraPreset = 0;

        GameManager.Instance.UpdateCameraMode(
            (CameraMode)GameManager.Instance.SelectedCameraPreset
        );
    }
}
