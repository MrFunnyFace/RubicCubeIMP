using UnityEngine;
using System.Collections.Generic;

public class RubikCubeGenerator : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    public Cubie cubiePrefab;
    public Material white, yellow, red, orange, blue, green;

    [Header("Highlight")]
    public GameObject highlightPrefab;

    [HideInInspector]
    public GameObject highlightInstance;


    [Header("Cube Settings")]
    [Range(2, 4)]
    public int cubeSize = 3;
    public float cubieSize = 1f;
    public float spacing = 0.05f;

    [Header("Visual Root")]
    public Transform cubeVisualRoot;

    [HideInInspector]
    public List<Cubie> cubies = new();

    [Header("Controler")]
    public RubikCubeController cubeController;
    public int moves = 20;

    private void Start()
    {
        if (GameManager.Instance != null)
            cubeSize = Mathf.Clamp(GameManager.Instance.SelectedCubeSize, 2, 4);

        GenerateCube();
    }

    public void GenerateCube()
    {
        ClearOldCube();

        int max = cubeSize - 1;
        float offset = (cubeSize - 1) / 2f;
        int id = 0;

        for (int x = 0; x < cubeSize; x++)
            for (int y = 0; y < cubeSize; y++)
                for (int z = 0; z < cubeSize; z++)
                {
                    Cubie c = Instantiate(cubiePrefab, cubeVisualRoot);
                    c.transform.localPosition = new Vector3(
                        (x - offset) * (cubieSize + spacing),
                        (y - offset) * (cubieSize + spacing),
                        (z - offset) * (cubieSize + spacing)
                    );
                    c.transform.localRotation = Quaternion.identity;
                    c.transform.localScale = Vector3.one * cubieSize;
                    c.Init(id,new Vector3Int(x, y, z));

                    if (x == 0) c.SetFaceColor(CubeFace.Left, blue);
                    if (x == max) c.SetFaceColor(CubeFace.Right, green);
                    if (y == 0) c.SetFaceColor(CubeFace.Down, yellow);
                    if (y == max) c.SetFaceColor(CubeFace.Up, white);
                    if (z == 0) c.SetFaceColor(CubeFace.Back, orange);
                    if (z == max) c.SetFaceColor(CubeFace.Front, red);

                    cubies.Add(c);
                    id++;
                }
        CreateHighlight();   // ВАЖНО
        if (cubeController != null)
        {
            cubeController.FastShuffle(moves, disableShuffle: false);
        }
    }
    void CreateHighlight()
    {

        if (highlightPrefab == null)
        {
            Debug.LogWarning("Highlight prefab is not assigned!");
            return;
        }

        if (highlightInstance != null)
            Destroy(highlightInstance);

        highlightInstance = Instantiate(highlightPrefab, cubeVisualRoot);
        highlightInstance.name = "Highlight";
        highlightInstance.transform.localPosition = Vector3.zero;
        highlightInstance.transform.localRotation = Quaternion.identity;
        highlightInstance.SetActive(false);

        Debug.Log("Highlight created under CubeVisualRoot");
    }

    private void ClearOldCube()
    {
        cubies.Clear();
        for (int i = cubeVisualRoot.childCount - 1; i >= 0; i--)
            Destroy(cubeVisualRoot.GetChild(i).gameObject);
    }

    public Vector3 GetLocalPositionFromIndex(Vector3Int index)
    {
        float offset = (cubeSize - 1) / 2f;
        return new Vector3(
            (index.x - offset) * (cubieSize + spacing),
            (index.y - offset) * (cubieSize + spacing),
            (index.z - offset) * (cubieSize + spacing)
        );
    }

    public Material GetMaterialByName(string name)
    {
        return name switch
        {
            "white" => white,
            "yellow" => yellow,
            "red" => red,
            "orange" => orange,
            "blue" => blue,
            "green" => green,
            _ => null
        };
    }
    //public void LoadCube(RubikCubeGenerator gen, SaveData data)
    //{
    //    gen.cubeSize = data.cubeSize;
    //    gen.GenerateCube();

    //    foreach (var c in data.cubeData.cubies)
    //    {
    //        var cubie = gen.cubies.Find(x =>
    //            x.Index == new Vector3Int(c.x, c.y, c.z));

    //        if (cubie == null) continue;

    //        cubie.transform.localPosition = c.localPosition;
    //        cubie.transform.localRotation = c.localRotation;

    //        cubie.SetFaceColor(CubeFace.Up, gen.GetMaterialByName(c.faceColors.up));
    //        cubie.SetFaceColor(CubeFace.Down, gen.GetMaterialByName(c.faceColors.down));
    //        cubie.SetFaceColor(CubeFace.Left, gen.GetMaterialByName(c.faceColors.left));
    //        cubie.SetFaceColor(CubeFace.Right, gen.GetMaterialByName(c.faceColors.right));
    //        cubie.SetFaceColor(CubeFace.Front, gen.GetMaterialByName(c.faceColors.front));
    //        cubie.SetFaceColor(CubeFace.Back, gen.GetMaterialByName(c.faceColors.back));
    //    }
    //}

}


