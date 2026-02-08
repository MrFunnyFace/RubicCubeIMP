using UnityEngine;
using TMPro;
public class WinPanel : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text timeText;

    private bool shown = false;

    void Start()
    {
        winPanel.SetActive(false);
    }

    void Update()
    {
        if (shown) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.CubeSolved)
        {
            ShowWin();
            shown = true;
        }
    }

    private void ShowWin()
    {
        GameManager.Instance.SetState(GameState.Win);
        GameManager.Instance.PauseGame(true);

        movesText.text = $"Ходы: {GameManager.Instance.MoveCount}";

        float time = GameManager.Instance.PlayTime;
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        timeText.text = $"Время: {min:00}:{sec:00}";

        winPanel.SetActive(true);
    }
}
