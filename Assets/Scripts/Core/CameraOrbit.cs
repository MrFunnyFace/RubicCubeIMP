using System.Collections;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public CameraMode cameraMode = CameraMode.PublicRotation;

    [SerializeField] private Transform cubeRoot;
    public Transform target;

    public float distance = 10f;
    public float rotateSpeed = 250f;
    public float maxFreeAngle = 45f;
    public float snapDuration = 0.25f;

    private bool isSnapping;

    private Vector3 baseForward;
    private Vector3 baseUp;

    private float yawOffset;
    private float pitchOffset;

    public Vector3 BaseForward => baseForward;
    public Vector3 BaseUp => baseUp;

    void Start()
    {
        baseForward = Vector3.back;
        baseUp = Vector3.up;
        ApplyTransform();
    }

    void Update()
    {
        HandleMouse();
        ApplyTransform();
    }

    public void SetMode(CameraMode mode)
    {
        cameraMode = mode;
        yawOffset = pitchOffset = 0;

        switch (mode)
        {
            case CameraMode.PublicRotation:
                // ===== НОВОЕ: всегда возвращаемся на центр стороны =====
                RecenterOnSide();
                break;

            case CameraMode.Fix:
                // Ничего не делаем, Fixed будет управляться отдельно
                break;

            case CameraMode.Izometria:
                SetIsometric();
                break;
        }

        ApplyTransform();
    }

    void RecenterOnSide()
    {
        // Выбираем ближайшую сторону куба к текущему положению камеры
        Vector3 localDir = cubeRoot.InverseTransformDirection((transform.position - cubeRoot.position).normalized);

        float absX = Mathf.Abs(localDir.x);
        float absY = Mathf.Abs(localDir.y);
        float absZ = Mathf.Abs(localDir.z);

        // Определяем главную ось (сторону), к которой камера ближе всего
        if (absX >= absY && absX >= absZ)
            baseForward = Vector3.right * Mathf.Sign(localDir.x);
        else if (absY >= absX && absY >= absZ)
            baseForward = Vector3.up * Mathf.Sign(localDir.y);
        else
            baseForward = Vector3.forward * Mathf.Sign(localDir.z);

        baseUp = Vector3.up; // вертикаль всегда вверх
    }

    void HandleMouse()
    {
        if (cameraMode != CameraMode.PublicRotation)
            return;
        if (!Input.GetMouseButton(1) || isSnapping)
            return;

        yawOffset += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        pitchOffset -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        yawOffset = Mathf.Clamp(yawOffset, -maxFreeAngle, maxFreeAngle);
        pitchOffset = Mathf.Clamp(pitchOffset, -maxFreeAngle, maxFreeAngle);
    }

    public void RotateLeft() => Snap(baseUp, -90);
    public void RotateRight() => Snap(baseUp, 90);
    public void RotateUp() => Snap(Vector3.Cross(baseUp, baseForward), -90);
    public void RotateDown() => Snap(Vector3.Cross(baseUp, baseForward), 90);
    public void RollScreenCW() => Snap(baseForward, 90);
    public void RollScreenCCW() => Snap(baseForward, -90);

    public CubeAxis GetScreenHorizontalAxis()
    {
        Vector3 screenRight = Vector3.Cross(baseUp, baseForward).normalized;
        return BestAxis(screenRight);
    }

    public CubeAxis GetScreenVerticalAxis()
    {
        return BestAxis(baseUp);
    }

    public CubeAxis BestAxis(Vector3 dir)
    {
        dir.Normalize();
        float x = Mathf.Abs(Vector3.Dot(dir, cubeRoot.right));
        float y = Mathf.Abs(Vector3.Dot(dir, cubeRoot.up));
        float z = Mathf.Abs(Vector3.Dot(dir, cubeRoot.forward));
        if (x > y && x > z) return CubeAxis.X;
        if (y > z) return CubeAxis.Y;
        return CubeAxis.Z;
    }

    public bool IsClockwise(Vector3 dir, CubeAxis axis)
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

    void Snap(Vector3 axis, float angle)
    {
        if (isSnapping)
            return;
        StartCoroutine(SnapCoroutine(axis, angle));
    }

    void SetIsometric()
    {
        baseForward = new Vector3(1, -1, 1).normalized;
        baseUp = Vector3.up;
        yawOffset = pitchOffset = 0;
        ApplyTransform();
    }

    IEnumerator SnapCoroutine(Vector3 axis, float angle)
    {
        isSnapping = true;
        Quaternion start = Quaternion.LookRotation(baseForward, baseUp);
        Quaternion end = Quaternion.AngleAxis(angle, axis) * start;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / snapDuration;
            Quaternion rot = Quaternion.Slerp(start, end, t);
            baseForward = rot * Vector3.forward;
            baseUp = rot * Vector3.up;
            yawOffset = pitchOffset = 0;
            ApplyTransform();
            yield return null;
        }
        baseForward = end * Vector3.forward;
        baseUp = end * Vector3.up;
        yawOffset = pitchOffset = 0;
        isSnapping = false;
    }

    void ApplyTransform()
    {
        Quaternion freeRot = Quaternion.AngleAxis(yawOffset, baseUp) *
                             Quaternion.AngleAxis(pitchOffset, Vector3.Cross(baseUp, baseForward));
        Vector3 dir = freeRot * baseForward;
        transform.position = target.position - dir * distance;
        transform.LookAt(target.position, baseUp);
    }
}


//using System.Collections;
//using UnityEngine;

//public class CameraOrbit : MonoBehaviour
//{
//    public CameraMode cameraMode = CameraMode.PublicRotation;

//    [SerializeField] private Transform cubeRoot;
//    public Transform target;

//    public float distance = 10f;
//    public float rotateSpeed = 250f;
//    public float maxFreeAngle = 45f;
//    public float snapDuration = 0.25f;

//    [System.Serializable]
//    public struct CameraPresetAngles
//    {
//        public float yaw;
//        public float pitch;
//        public float distance;
//    }
//    [SerializeField] private CameraPresetAngles[] fixedPresets;
//    private int currentPresetIndex = 0;

//    private bool isSnapping;

//    // ===== Логика куба (для CubeCameraInput) =====
//    private Vector3 baseForward = Vector3.back;
//    private Vector3 baseUp = Vector3.up;

//    // ===== Визуал камеры =====
//    private Vector3 camForward;
//    private Vector3 camUp;
//    private float yawOffset;
//    private float pitchOffset;
//    private float camDistance;

//    public Vector3 BaseForward => baseForward;
//    public Vector3 BaseUp => baseUp;

//    void Start()
//    {
//        if (cubeRoot == null) Debug.LogError("CubeRoot не назначен!");
//        camDistance = distance;
//        camForward = (transform.position - cubeRoot.position).normalized;
//        camUp = Vector3.up;
//        ApplyTransform();
//    }

//    void Update()
//    {
//        HandleMouse();
//        ApplyTransform();
//    }

//    void HandleMouse()
//    {
//        if (cameraMode != CameraMode.PublicRotation || isSnapping) return;
//        if (!Input.GetMouseButton(1)) return;

//        yawOffset += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
//        pitchOffset -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

//        yawOffset = Mathf.Clamp(yawOffset, -maxFreeAngle, maxFreeAngle);
//        pitchOffset = Mathf.Clamp(pitchOffset, -maxFreeAngle, maxFreeAngle);
//    }

//    public void SetMode(CameraMode mode)
//    {
//        cameraMode = mode;
//        yawOffset = pitchOffset = 0;

//        switch (mode)
//        {
//            case CameraMode.PublicRotation:
//                // Свободный режим — используем текущее положение камеры
//                camForward = (transform.position - cubeRoot.position).normalized;
//                camUp = Vector3.up;
//                camDistance = distance;
//                break;

//            case CameraMode.Izometria:
//                camForward = new Vector3(1, -1, 1).normalized;
//                camUp = Vector3.up;
//                camDistance = 10f;
//                break;

//            case CameraMode.Fix:
//                ApplyFixedPreset(currentPresetIndex);
//                break;
//        }

//        ApplyTransform();
//    }

//    public void NextFixedPreset()
//    {
//        currentPresetIndex = (currentPresetIndex + 1) % fixedPresets.Length;
//        ApplyFixedPreset(currentPresetIndex);
//    }

//    public void PreviousFixedPreset()
//    {
//        currentPresetIndex--;
//        if (currentPresetIndex < 0) currentPresetIndex = fixedPresets.Length - 1;
//        ApplyFixedPreset(currentPresetIndex);
//    }

//    void ApplyFixedPreset(int index)
//    {
//        if (cubeRoot == null || index < 0 || index >= fixedPresets.Length) return;

//        CameraPresetAngles preset = fixedPresets[index];
//        float yawRad = Mathf.Deg2Rad * preset.yaw;
//        float pitchRad = Mathf.Deg2Rad * preset.pitch;

//        Vector3 dir;
//        dir.x = Mathf.Cos(pitchRad) * Mathf.Sin(yawRad);
//        dir.y = Mathf.Sin(pitchRad);
//        dir.z = Mathf.Cos(pitchRad) * Mathf.Cos(yawRad);

//        camForward = dir.normalized;
//        camUp = Vector3.up;
//        camDistance = preset.distance;

//        ApplyTransform();
//    }

//    void ApplyTransform()
//    {
//        Quaternion rot = Quaternion.AngleAxis(yawOffset, camUp) *
//                         Quaternion.AngleAxis(pitchOffset, Vector3.Cross(camUp, camForward));

//        Vector3 dir = rot * camForward;
//        transform.position = cubeRoot.position + dir * camDistance;
//        transform.LookAt(cubeRoot.position, camUp);
//    }

//    public void RotateLeft() => Snap(camUp, -90);
//    public void RotateRight() => Snap(camUp, 90);
//    public void RotateUp() => Snap(Vector3.Cross(camUp, camForward), -90);
//    public void RotateDown() => Snap(Vector3.Cross(camUp, camForward), 90);
//    public void RollScreenCW() => Snap(camForward, 90);
//    public void RollScreenCCW() => Snap(camForward, -90);

//    public CubeAxis GetScreenHorizontalAxis() => BestAxis(Vector3.Cross(camUp, camForward));
//    public CubeAxis GetScreenVerticalAxis() => BestAxis(camUp);

//    public CubeAxis BestAxis(Vector3 dir)
//    {
//        dir.Normalize();
//        float x = Mathf.Abs(Vector3.Dot(dir, cubeRoot.right));
//        float y = Mathf.Abs(Vector3.Dot(dir, cubeRoot.up));
//        float z = Mathf.Abs(Vector3.Dot(dir, cubeRoot.forward));
//        if (x > y && x > z) return CubeAxis.X;
//        if (y > z) return CubeAxis.Y;
//        return CubeAxis.Z;
//    }

//    public bool IsClockwise(Vector3 dir, CubeAxis axis)
//    {
//        Vector3 axisVec = axis switch
//        {
//            CubeAxis.X => cubeRoot.right,
//            CubeAxis.Y => cubeRoot.up,
//            CubeAxis.Z => cubeRoot.forward,
//            _ => Vector3.up
//        };
//        return Vector3.Dot(dir, axisVec) > 0f;
//    }

//    void Snap(Vector3 axis, float angle)
//    {
//        if (isSnapping) return;
//        StartCoroutine(SnapCoroutine(axis, angle));
//    }

//    IEnumerator SnapCoroutine(Vector3 axis, float angle)
//    {
//        isSnapping = true;
//        Quaternion start = Quaternion.LookRotation(camForward, camUp);
//        Quaternion end = Quaternion.AngleAxis(angle, axis) * start;
//        float t = 0f;
//        while (t < 1f)
//        {
//            t += Time.deltaTime / snapDuration;
//            Quaternion rot = Quaternion.Slerp(start, end, t);
//            camForward = rot * Vector3.forward;
//            camUp = rot * Vector3.up;
//            yawOffset = pitchOffset = 0;
//            ApplyTransform();
//            yield return null;
//        }
//        camForward = end * Vector3.forward;
//        camUp = end * Vector3.up;
//        yawOffset = pitchOffset = 0;
//        isSnapping = false;
//    }

//    void SetIsometric()
//    {
//        camForward = new Vector3(1, -1, 1).normalized;
//        camUp = Vector3.up;
//        yawOffset = pitchOffset = 0;
//        ApplyTransform();
//    }
//}
