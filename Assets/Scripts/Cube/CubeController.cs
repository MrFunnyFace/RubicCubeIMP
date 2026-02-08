using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RubikCubeController : MonoBehaviour
{
    public float rotationDuration = 0.3f;
    public GameObject highlightPrefab;
    private bool isRotating;
    private RubikCubeGenerator generator;
    //private GameObject highlightInstance;
    private bool hasUserMoved = false;
    public Cubie SelectedCubie;
    //[Header("Shufl")]
    //public int moves = 2;
    //public bool shufle = true;

    [SerializeField]
    public WinPanel winPanel;
    void Awake() => generator = GetComponent<RubikCubeGenerator>();

    //void Start()
    //{
    //    StartCoroutine(InitHighlightWhenReady());
    //}

    //IEnumerator InitHighlightWhenReady()
    //{
    //    // ждём, пока генератор создаст визуальный рут
    //    while (generator == null || generator.cubeVisualRoot == null)
    //        yield return null;

    //    highlightInstance = Instantiate(
    //        highlightPrefab,
    //        generator.cubeVisualRoot
    //    );
    //    highlightInstance.name = "Highlight";
    //    highlightInstance.SetActive(false);

    //    Debug.Log("Highlight created");
    //}

    void Update()
    {
        HandleSelection();
        UpdateHighlight();
    }

    void HandleSelection()
    {

        if (isRotating) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Cubie cubie = hit.collider.GetComponentInParent<Cubie>();
        if (cubie == null) return;

        SelectedCubie?.Deselect();
        SelectedCubie = cubie;
        SelectedCubie.Select();
    }

    void UpdateHighlight()
    {
        if (generator == null || generator.highlightInstance == null)
            return;

        var highlight = generator.highlightInstance;

        if (SelectedCubie == null)
        {
            highlight.SetActive(false);
            return;
        }

        highlight.SetActive(true);

        // центр выбранного кубика
        highlight.transform.localPosition =
            generator.GetLocalPositionFromIndex(SelectedCubie.Index);

        highlight.transform.localRotation = Quaternion.identity;

        float scale = generator.cubieSize * 1.05f;
        highlight.transform.localScale = Vector3.one * scale;
    }



    public void Rotate(CubeAxis axis, bool clockwise)
    {
        if (isRotating || SelectedCubie == null) return;
        hasUserMoved = true;
        int layer = axis switch
        {
            CubeAxis.X => SelectedCubie.Index.x,
            CubeAxis.Y => SelectedCubie.Index.y,
            CubeAxis.Z => SelectedCubie.Index.z,
            _ => 0
        };

        StartCoroutine(RotateLayer(axis, layer, clockwise));

    }

    IEnumerator RotateLayer(CubeAxis axis, int layer, bool clockwise)
    {
        isRotating = true;

        List<Cubie> layerCubies = GetLayerCubies(axis, layer);

        //Debug.Log($"=== ROTATING LAYER {axis}={layer} CW={clockwise} ===");
        //Debug.Log("Before rotation:");
        //foreach (var c in layerCubies)
        //    Debug.Log($"Cubie ID={c.ID} Index={c.Index}");

        GameObject pivot = new GameObject("Pivot");
        pivot.transform.SetParent(generator.cubeVisualRoot, false);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = Quaternion.identity;

        foreach (var c in layerCubies)
            c.transform.SetParent(pivot.transform, true);

        Vector3 axisVec = axis switch
        {
            CubeAxis.X => Vector3.right,
            CubeAxis.Y => Vector3.up,
            CubeAxis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        // 🔹 Если X, то используем новую реализацию (CW +90°)
        float angle = axis == CubeAxis.X
            ? (clockwise ? 90f : -90f)
            : (clockwise ? -90f : 90f); // Y и Z остаются как раньше

        Quaternion start = Quaternion.identity;
        Quaternion end = Quaternion.AngleAxis(angle, axisVec);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotationDuration;
            pivot.transform.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
        pivot.transform.localRotation = end;

        // 🔹 Обновление индексов
        if (axis == CubeAxis.X)
        {
            // только X по новой формуле
            int max = generator.cubeSize - 1;
            foreach (var c in layerCubies)
            {
                Vector3Int i = c.Index;
                Vector3Int newIndex = clockwise
                    ? new Vector3Int(i.x, max - i.z, i.y)
                    : new Vector3Int(i.x, i.z, max - i.y);
                c.SetIndex(newIndex);
            }
        }
        else
        {
            // Y и Z по старой реализации
            UpdateIndices(layerCubies, axis, clockwise);
        }

        UpdateCubieFaces(layerCubies, axis, clockwise);

        //Debug.Log("After rotation:");
        //foreach (var c in layerCubies)
        //    Debug.Log($"Cubie ID={c.ID} New Index={c.Index}");
        //Debug.Log("============================");

        // Фиксация после анимации
        foreach (var c in layerCubies)
        {
            c.transform.SetParent(generator.cubeVisualRoot, true);
            c.transform.localPosition = generator.GetLocalPositionFromIndex(c.Index);
            c.transform.localRotation = Quaternion.identity;
        }

        Destroy(pivot);
        isRotating = false;
        CheckCubeState();
    }





    void UpdateCubieFaces(List<Cubie> cubies, CubeAxis axis, bool clockwise)
    {
        foreach (var c in cubies)
        {
            if (axis == CubeAxis.Y)
            {
                if (clockwise)
                {
                    var front = c.GetFace(CubeFace.Front).renderer.material;
                    var right = c.GetFace(CubeFace.Right).renderer.material;
                    var back = c.GetFace(CubeFace.Back).renderer.material;
                    var left = c.GetFace(CubeFace.Left).renderer.material;

                    c.GetFace(CubeFace.Front).renderer.material = right;
                    c.GetFace(CubeFace.Right).renderer.material = back;
                    c.GetFace(CubeFace.Back).renderer.material = left;
                    c.GetFace(CubeFace.Left).renderer.material = front;
                }
                else
                {

                    var front = c.GetFace(CubeFace.Front).renderer.material;
                    var right = c.GetFace(CubeFace.Right).renderer.material;
                    var back = c.GetFace(CubeFace.Back).renderer.material;
                    var left = c.GetFace(CubeFace.Left).renderer.material;

                    c.GetFace(CubeFace.Front).renderer.material = left;
                    c.GetFace(CubeFace.Right).renderer.material = front;
                    c.GetFace(CubeFace.Back).renderer.material = right;
                    c.GetFace(CubeFace.Left).renderer.material = back;

                }
            }
            if (axis == CubeAxis.X)
            {
                if (clockwise)
                {
                    var up = c.GetFace(CubeFace.Up).renderer.material;
                    var back = c.GetFace(CubeFace.Back).renderer.material;
                    var down = c.GetFace(CubeFace.Down).renderer.material;
                    var front = c.GetFace(CubeFace.Front).renderer.material;

                    c.GetFace(CubeFace.Up).renderer.material = back;
                    c.GetFace(CubeFace.Front).renderer.material = up;
                    c.GetFace(CubeFace.Down).renderer.material = front;
                    c.GetFace(CubeFace.Back).renderer.material = down;

                }
                else
                {
                    var up = c.GetFace(CubeFace.Up).renderer.material;
                    var back = c.GetFace(CubeFace.Back).renderer.material;
                    var down = c.GetFace(CubeFace.Down).renderer.material;
                    var front = c.GetFace(CubeFace.Front).renderer.material;

                    c.GetFace(CubeFace.Up).renderer.material = front;
                    c.GetFace(CubeFace.Back).renderer.material = up;
                    c.GetFace(CubeFace.Down).renderer.material = back;
                    c.GetFace(CubeFace.Front).renderer.material = down;
                }
            }
            if (axis == CubeAxis.Z)
            {
                if (clockwise)
                {
                    var left = c.GetFace(CubeFace.Left).renderer.material;
                    var up = c.GetFace(CubeFace.Up).renderer.material;
                    var right = c.GetFace(CubeFace.Right).renderer.material;
                    var down = c.GetFace(CubeFace.Down).renderer.material;

                    c.GetFace(CubeFace.Up).renderer.material = left;
                    c.GetFace(CubeFace.Right).renderer.material = up;
                    c.GetFace(CubeFace.Down).renderer.material = right;
                    c.GetFace(CubeFace.Left).renderer.material = down;
                }
                else
                {
                    var left = c.GetFace(CubeFace.Left).renderer.material;
                    var up = c.GetFace(CubeFace.Up).renderer.material;
                    var right = c.GetFace(CubeFace.Right).renderer.material;
                    var down = c.GetFace(CubeFace.Down).renderer.material;

                    c.GetFace(CubeFace.Up).renderer.material = right;
                    c.GetFace(CubeFace.Left).renderer.material = up;
                    c.GetFace(CubeFace.Down).renderer.material = left;
                    c.GetFace(CubeFace.Right).renderer.material = down;
                }
            }
        }
    }
    List<Cubie> GetLayerCubies(CubeAxis axis, int layer)
    {
        return generator.cubies.FindAll(c =>
            axis switch
            {
                CubeAxis.X => c.Index.x == layer,
                CubeAxis.Y => c.Index.y == layer,
                CubeAxis.Z => c.Index.z == layer,
                _ => false
            });
    }
    void UpdateIndices(List<Cubie> cubies, CubeAxis axis, bool cw)
    {
        int max = generator.cubeSize - 1;

        foreach (var c in cubies)
        {
            Vector3Int i = c.Index;
            Vector3Int newIndex = axis switch
            {
                CubeAxis.X => cw ? new Vector3Int(i.x, max - i.z, i.y) : new Vector3Int(i.x, i.z, max - i.y),
                CubeAxis.Y => cw ? new Vector3Int(max - i.z, i.y, i.x) : new Vector3Int(i.z, i.y, max - i.x),
                CubeAxis.Z => cw ? new Vector3Int(i.y, max - i.x, i.z) : new Vector3Int(max - i.y, i.x, i.z),
                _ => i
            };
            c.SetIndex(newIndex);
        }
    }
    public void FastShuffle(int moves = 20, bool disableShuffle = false)
    {
        if (disableShuffle) return;
        if (generator == null || generator.cubies == null || generator.cubies.Count == 0)
            return; // Куб еще не сгенерирован

        System.Random rand = new System.Random();
        CubeAxis[] axes = { CubeAxis.X, CubeAxis.Y, CubeAxis.Z };

        for (int i = 0; i < moves; i++)
        {
            CubeAxis axis = axes[rand.Next(0, 3)];
            bool cw = rand.Next(0, 2) == 0; // true/false случайно
            int layer = rand.Next(0, generator.cubeSize);

            // Вызов ускоренного вращения без анимации
            RotateLayerImmediate(axis, layer, cw);
        }

        // Если есть GameManager, сбрасываем флаг "Кубик собран"
        if (GameManager.Instance != null)
            GameManager.Instance.CubeSolved = false;
    }
    public void RotateLayerImmediate(CubeAxis axis, int layer, bool clockwise)
    {
        List<Cubie> layerCubies = GetLayerCubies(axis, layer);

        GameObject pivot = new GameObject("Pivot");
        pivot.transform.SetParent(generator.cubeVisualRoot, false);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = Quaternion.identity;

        foreach (var c in layerCubies)
            c.transform.SetParent(pivot.transform, true);

        // Вращение сразу
        Vector3 axisVec = axis switch
        {
            CubeAxis.X => Vector3.right,
            CubeAxis.Y => Vector3.up,
            CubeAxis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        float angle = axis == CubeAxis.X
            ? (clockwise ? 90f : -90f)
            : (clockwise ? -90f : 90f);

        pivot.transform.localRotation = Quaternion.AngleAxis(angle, axisVec);

        // Обновление индексов
        if (axis == CubeAxis.X)
        {
            int max = generator.cubeSize - 1;
            foreach (var c in layerCubies)
            {
                Vector3Int i = c.Index;
                Vector3Int newIndex = clockwise
                    ? new Vector3Int(i.x, max - i.z, i.y)
                    : new Vector3Int(i.x, i.z, max - i.y);
                c.SetIndex(newIndex);
            }
        }
        else
        {
            UpdateIndices(layerCubies, axis, clockwise);
        }

        UpdateCubieFaces(layerCubies, axis, clockwise);

        // Фиксация после вращения
        foreach (var c in layerCubies)
        {
            c.transform.SetParent(generator.cubeVisualRoot, true);
            c.transform.localPosition = generator.GetLocalPositionFromIndex(c.Index);
            c.transform.localRotation = Quaternion.identity;
        }

        Destroy(pivot);
    }

    public void CheckCubeState()
    {
        if (generator == null || generator.cubies.Count == 0)
            return;
        if (!hasUserMoved)
            return;

        int cubeSize = generator.cubeSize;
        Dictionary<CubeFace, bool> sideComplete = new Dictionary<CubeFace, bool>();

        foreach (CubeFace face in System.Enum.GetValues(typeof(CubeFace)))
            sideComplete[face] = true;

        foreach (CubeFace face in System.Enum.GetValues(typeof(CubeFace)))
        {
            Material reference = null;

            foreach (var cubie in generator.cubies)
            {
                Vector3Int idx = cubie.Index;
                bool relevantCubie = face switch
                {
                    CubeFace.Up => idx.y == cubeSize - 1,
                    CubeFace.Down => idx.y == 0,
                    CubeFace.Left => idx.x == 0,
                    CubeFace.Right => idx.x == cubeSize - 1,
                    CubeFace.Front => idx.z == cubeSize - 1,
                    CubeFace.Back => idx.z == 0,
                    _ => false
                };

                if (!relevantCubie) continue;

                Material mat = cubie.GetFace(face).renderer.material;
                if (reference == null) reference = mat;
                else if (reference.color != mat.color)
                {
                    sideComplete[face] = false;
                    break;
                }
            }
        }

        bool allSolved = true;
        foreach (var solved in sideComplete.Values)
            if (!solved) allSolved = false;

        if (allSolved)
        {
            Debug.Log($"[WIN CHECK] allSolved = true | Time={Time.time}");

            if (GameManager.Instance == null)
            {
                Debug.LogError("[WIN CHECK] GameManager.Instance == null");
                return;
            }

            if (GameManager.Instance.CubeSolved)
            {
                Debug.Log("[WIN CHECK] CubeSolved already true → skip");
                return;
            }

            GameManager.Instance.CubeSolved = true;
            Debug.Log("[WIN CHECK] CubeSolved set to TRUE");
        }

        if (allSolved)
        {
            Debug.Log("Кубик полностью собран!");

            if (GameManager.Instance != null)
                GameManager.Instance.CubeSolved = true;
        }
    }


    public void RotateX_CW() => Rotate(CubeAxis.X, true);
    public void RotateX_CCW() => Rotate(CubeAxis.X, false);
    public void RotateY_CW() => Rotate(CubeAxis.Y, true);
    public void RotateY_CCW() => Rotate(CubeAxis.Y, false);
    public void RotateZ_CW() => Rotate(CubeAxis.Z, true);
    public void RotateZ_CCW() => Rotate(CubeAxis.Z, false);


}