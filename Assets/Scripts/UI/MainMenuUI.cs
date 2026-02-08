using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainMenuGroup;

    public void SetOverlayActive(bool active)
    {
        //mainMenuGroup.alpha = active ? 0.3f : 1f;
        mainMenuGroup.interactable = !active;
        mainMenuGroup.blocksRaycasts = !active;
    }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject loadPanel;
    public GameObject helpPanel;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = Object.FindFirstObjectByType<SceneLoader>();
        SetOverlayActive(false);
        ShowMainMenu();
    }

    private void HideAllOverlays()
    {
        settingsPanel.SetActive(false);
        loadPanel.SetActive(false);
        helpPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAllOverlays();
        SetOverlayActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnStartGame()
    {
        SetOverlayActive(false);
        GameManager.Instance.SetState(GameState.GameScene);
        sceneLoader.LoadGame();
    }

    public void OnLoadGame()
    {
        HideAllOverlays();
        SetOverlayActive(true);
        loadPanel.SetActive(true);
    }

    public void OnSettings()
    {
        HideAllOverlays();
        SetOverlayActive(true);
        settingsPanel.SetActive(true);
    }

    public void OnHelp()
    {
        HideAllOverlays();
        SetOverlayActive(true);
        helpPanel.SetActive(true);
    }

    public void OnQuit()
    {
        sceneLoader.QuitGame();
    }
}

