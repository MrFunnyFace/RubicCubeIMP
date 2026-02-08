using System.Linq;
using UnityEngine;

public class CubeCameraInput : MonoBehaviour
{
    [SerializeField] private CameraOrbit cameraOrbit;
    [SerializeField] private RubikCubeController cubeController;
    [SerializeField] private Transform cubeRoot;

    // Текущий выбранный кубик
    public Cubie SelectedCubie => cubeController.SelectedCubie;

    // ================= Камера =================

    public void RotateScreenLeft() => ResolveAndRotate(-GetScreenRight());
    public void RotateScreenRight() => ResolveAndRotate(GetScreenRight());
    public void RotateScreenUp() => ResolveAndRotate(GetScreenUp());
    public void RotateScreenDown() => ResolveAndRotate(-GetScreenUp());
    public void RotateScreenClockwise()
    {
        ResolveAndRotateAlongCamera(true);
    }
    public void RotateScreenCounterClockwise()
    {
        ResolveAndRotateAlongCamera(false);
    }

    void ResolveAndRotateAlongCamera(bool clockwise)
    {
        Vector3 camDir = (cubeRoot.position - cameraOrbit.transform.position).normalized;
        CubeAxis axis = ResolveAxis(camDir);
        bool isClockwise = clockwise;

        cubeController.Rotate(axis, isClockwise);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MoveCount++;
        }
    }

    Vector3 GetScreenUp() => cameraOrbit.BaseUp;
    Vector3 GetScreenRight() => Vector3.Cross(cameraOrbit.BaseUp, cameraOrbit.BaseForward).normalized;

    void ResolveAndRotate(Vector3 screenDir)
    {
        CubeAxis axis = ResolveAxis(screenDir);
        bool clockwise = ResolveClockwise(screenDir, axis);
        cubeController.Rotate(axis, clockwise);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MoveCount++;
        }
    }

    CubeAxis ResolveAxis(Vector3 dir)
    {
        float x = Mathf.Abs(Vector3.Dot(dir, cubeRoot.right));
        float y = Mathf.Abs(Vector3.Dot(dir, cubeRoot.up));
        float z = Mathf.Abs(Vector3.Dot(dir, cubeRoot.forward));

        if (x > y && x > z) return CubeAxis.X;
        if (y > z) return CubeAxis.Y;
        return CubeAxis.Z;
    }

    bool ResolveClockwise(Vector3 dir, CubeAxis axis)
    {
        Vector3 axisVec = axis switch
        {
            CubeAxis.X => cubeRoot.right,
            CubeAxis.Y => cubeRoot.up,
            CubeAxis.Z => cubeRoot.forward,
            _ => Vector3.up
        };
        return Vector3.Dot(dir, axisVec) > 0f;
    }
}
