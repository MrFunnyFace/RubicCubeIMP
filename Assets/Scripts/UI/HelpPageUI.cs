using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum HelpSection
{
    Purpose,
    Cube2,
    Cube3,
    Cube4
}

public class HelpPageUI : MonoBehaviour
{
    [Header("Main menu panel")]
    public MainMenuUI mainMenu;
    public void OnBack()
    {
        gameObject.SetActive(false);
        mainMenu.SetOverlayActive(false);
    }

    [Header("Scroll View")]
    [SerializeField] private Transform contentRoot;

    [Header("Content Prefabs")]
    [SerializeField] private GameObject ContentPurpose;
    [SerializeField] private GameObject Content2x2;
    [SerializeField] private GameObject Content3x3;
    [SerializeField] private GameObject Content4x4;

    [Header("Tabs")]
    [SerializeField] private Button purposeButton;
    [SerializeField] private Button cube2Button;
    [SerializeField] private Button cube3Button;
    [SerializeField] private Button cube4Button;

    [Header("Colors")]
    public Color activeTabColor = Color.white;
    public Color inactiveTabColor = new Color(1, 1, 1, 0.5f);

    private GameObject currentContent;


    private void Start()
    {
        ShowPurpose();
    }


    public void ShowPurpose() => Show(ContentPurpose, HelpSection.Purpose);
    public void Show2x2() => Show(Content2x2, HelpSection.Cube2);
    public void Show3x3() => Show(Content3x3, HelpSection.Cube3);
    public void Show4x4() => Show(Content4x4, HelpSection.Cube4);


    private void Show(GameObject prefab, HelpSection section)
    {
        if (prefab == null)
        {
            Debug.LogError($"Prefab for section {section} is NULL");
            return;
        }

        // Удаляем предыдущий контент
        if (currentContent != null)
            Destroy(currentContent);

        // Создаём новый
        currentContent = Instantiate(prefab, contentRoot);
        currentContent.transform.localScale = Vector3.one;

        UpdateTabs(section);
    }

    private void UpdateTabs(HelpSection active)
    {
        SetButtonState(purposeButton, active == HelpSection.Purpose);
        SetButtonState(cube2Button, active == HelpSection.Cube2);
        SetButtonState(cube3Button, active == HelpSection.Cube3);
        SetButtonState(cube4Button, active == HelpSection.Cube4);

    }

    private void SetButtonState(Button button, bool active)
    {
        if (button == null) return;

        var img = button.GetComponent<Image>();
        img.color = active ? activeTabColor : inactiveTabColor;
        //var colors = button.colors;
        //colors.normalColor = active ? Color.white : new Color(1, 1, 1, 0.5f);
        //button.colors = colors;
    }


}
