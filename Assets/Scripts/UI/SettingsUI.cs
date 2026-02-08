using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public MainMenuUI mainMenu;


    [Header("UI")]
    public Dropdown gameModeDropdown;
    public Toggle soundToggle;
    public Toggle timerToggle;
    public Toggle movesToggle;
    public Toggle hintsToggle;
    public Toggle highlightToggle;

    private void Start()
    {
        LoadFromGameManager();
    }

    private bool isInitializing;

    private void LoadFromGameManager()
    {
        isInitializing = true;

        var gm = GameManager.Instance;

        gameModeDropdown.value = (int)gm.gameMode;
        soundToggle.isOn = gm.soundEnabled;
        timerToggle.isOn = gm.timerEnabled;
        movesToggle.isOn = gm.movesCounterEnabled;
        hintsToggle.isOn = gm.hintsEnabled;
        highlightToggle.isOn = gm.highlightFacesEnabled;

        UpdateTutorialOptionsVisibility();

        isInitializing = false;
    }

    public void OnGameModeChanged(int value)
    {
        if (isInitializing) return;

        GameManager.Instance.SetGameMode((GameMode)value);
        LoadFromGameManager();
    }

    public void OnSoundChanged(bool value)
    {
        if (isInitializing) return;
        GameManager.Instance.soundEnabled = value;
    }

    public void OnTimerChanged(bool value)
    {
        if (isInitializing) return;
        GameManager.Instance.timerEnabled = value;
    }

    public void OnMovesChanged(bool value)
    {
        if (isInitializing) return;
        GameManager.Instance.movesCounterEnabled = value;
    }

    public void OnHintsChanged(bool value)
    {
        if (isInitializing) return;
        GameManager.Instance.hintsEnabled = value;
    }

    public void OnHighlightChanged(bool value)
    {
        if (isInitializing) return;
        GameManager.Instance.highlightFacesEnabled = value;
    }

    private void UpdateTutorialOptionsVisibility()
    {
        bool tutorial = GameManager.Instance.gameMode == GameMode.Tutorial;

        hintsToggle.gameObject.SetActive(tutorial);
        highlightToggle.gameObject.SetActive(tutorial);
    }
    public void OnBack()
    {
        var gm = GameManager.Instance;

        Debug.Log(
            $"[SETTINGS SAVED]\n" +
            $"Mode: {gm.gameMode}\n" +
            $"Sound: {gm.soundEnabled}\n" +
            $"Timer: {gm.timerEnabled}\n" +
            $"Moves: {gm.movesCounterEnabled}\n" +
            $"Hints: {gm.hintsEnabled}\n" +
            $"Highlight: {gm.highlightFacesEnabled}"
        );

        gameObject.SetActive(false);
        mainMenu.SetOverlayActive(false);
    }



}

