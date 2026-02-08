using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Top Bar")]
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text cameraModeText;

    [Header("Panels")]
    [SerializeField] 
    private GameObject pausePanel;


    private void Start()
    {
        UpdateAll();

    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        UpdateMoves();
        UpdateTime();
        UpdateCameraMode();
    }

    private void UpdateAll()
    {
        UpdateMoves();
        UpdateTime();
        UpdateCameraMode();
    }

    private void UpdateMoves()
    {
        if (!GameManager.Instance.movesCounterEnabled)
        {
            movesText.gameObject.SetActive(false);
            return;
        }

        movesText.gameObject.SetActive(true);
        movesText.text = $"Ходы: {GameManager.Instance.MoveCount}";
    }

    private void UpdateTime()
    {
        if (!GameManager.Instance.timerEnabled)
        {
            timeText.gameObject.SetActive(false);
            return;
        }

        timeText.gameObject.SetActive(true);

        float time = GameManager.Instance.PlayTime;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    [SerializeField] private GameObject nextPresetButton;
    [SerializeField] private GameObject prevPresetButton;

    private void UpdateCameraMode()
    {
        cameraModeText.text = $"Ракурс: {GetCameraModeName()}";

        bool showFixedButtons = GameManager.Instance.SelectedCameraPreset == (int)CameraMode.Fix;
        nextPresetButton.SetActive(showFixedButtons);
        prevPresetButton.SetActive(showFixedButtons);
    }


    private string GetCameraModeName()
    {
        return GameManager.Instance.SelectedCameraPreset switch
        {
            0 => "Свободный",
            1 => "Фиксированный",
            2 => "Изометрия",
            _ => "Неизвестно"
        };
    }

    public void OnPausePressed()
    {
        pausePanel.SetActive(true);
        GameManager.Instance.PauseGame(true);
    }

    public void OnChangeCameraMode()
    {
        GameManager.Instance.SelectedCameraPreset++;
        if (GameManager.Instance.SelectedCameraPreset > 2)
            GameManager.Instance.SelectedCameraPreset = 0;
    }
}

