using UnityEngine;
using TMPro;

public class CubeSizeSelectorUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [Header("Preview")]
    [SerializeField] private UICubeCameraController previewController;

    private int currentIndex = 0;
    private readonly string[] sizes = { "2x2x2", "3x3x3", "4x4x4" };

    private void Start()
    {
        UpdateView();
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % sizes.Length;
        UpdateView();
    }

    public void Prev()
    {
        currentIndex = (currentIndex - 1 + sizes.Length) % sizes.Length;
        UpdateView();
    }

    private void UpdateView()
    {
        int size = GetSelectedSize();
        GameManager.Instance.SelectedCubeSize = size;

        if (sizeText != null)
            sizeText.text = sizes[currentIndex];

        if (previewController == null || previewController.cubeGenerator == null)
        {
            Debug.LogWarning("CubeSizeSelectorUI: previewController или cubeGenerator не назначены");
            return;
        }

        // 1. Пересоздаём куб
        previewController.cubeGenerator.cubeSize = size;
        previewController.cubeGenerator.GenerateCube();
    }

    public int GetSelectedSize()
    {
        return currentIndex + 2; // 2,3,4
    }
}


