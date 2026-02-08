using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FaceGridUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private FaceButtonUI buttonPrefab;

    private readonly List<FaceButtonUI> spawnedButtons = new();

    private void Start()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        if (grid == null || buttonPrefab == null)
        {
            Debug.LogError("FaceGridUI: не назначены ссылки в инспекторе");
            return;
        }

        ClearGrid();

        int cubeSize = GameManager.Instance != null
            ? GameManager.Instance.SelectedCubeSize
            : 3;

        int faceCount = cubeSize * cubeSize;

        Debug.Log($"FaceGridUI: создаём {faceCount} кнопок для куба {cubeSize}x{cubeSize}");

        for (int i = 0; i < faceCount; i++)
        {
            var button = Instantiate(buttonPrefab, grid.transform);
            button.Setup(i, OnFaceButtonClicked);
            spawnedButtons.Add(button);
        }
    }

    private void ClearGrid()
    {
        foreach (var btn in spawnedButtons)
            Destroy(btn.gameObject);

        spawnedButtons.Clear();
    }

    private void OnFaceButtonClicked(int index)
    {
        Debug.Log($"[FaceGridUI] Выбрана ячейка грани: {index}");

        //TODO: ЗАГЛУШКА
        // В будущем RubikCubeController.SelectFaceCell(index);
    }
}


