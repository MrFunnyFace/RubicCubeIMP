using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int SelectedCubeSize { get; set; } = 3;
    public GameMode SelectedGameMode { get; set; } = GameMode.Normal;
    public bool HintsEnabled { get; set; } = false;
    public int SelectedCameraPreset { get; set; } = 0;

    public bool IsLoadingGame { get; set; } = false;
    public string SelectedSaveId { get; set; }

    public CameraMode camMode { get; set; }
    public int MoveCount { get; set; }
    public float PlayTime { get; set; }

    public GameState CurrentState { get; private set; }

    [Header("Game Mode")]
    public GameMode gameMode = GameMode.Normal;

    [Header("Settings")]
    public bool soundEnabled = true;
    public bool timerEnabled = true;
    public bool movesCounterEnabled = true;
    public bool hintsEnabled = false;
    public bool highlightFacesEnabled = false;
    public bool CubeSolved = false;


    public bool isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isPaused) return;
        if (CurrentState != GameState.GameScene) return;

        PlayTime += Time.deltaTime;
    }


    public void PauseGame(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }

    public void UpdateCameraMode(CameraMode mode)
    {
        camMode = mode;

        CameraOrbit cam = FindFirstObjectByType<CameraOrbit>();
        if (cam != null)
            cam.SetMode(mode);
    }


    public void SetGameMode(GameMode mode)
    {
        gameMode = mode;
        ApplyGameModeDefaults();
    }

    private void ApplyGameModeDefaults()
    {
        if (gameMode == GameMode.Normal)
        {
            hintsEnabled = false;
            highlightFacesEnabled = false;
        }
        else if (gameMode == GameMode.Tutorial)
        {
            hintsEnabled = true;
            highlightFacesEnabled = true;
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game state changed to: " + newState);
    }

    public void ResetSession()
    {
        MoveCount = 0;
        PlayTime = 0f;
        CubeSolved = false;
        PauseGame(false);
    }

}
