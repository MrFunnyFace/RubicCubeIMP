using UnityEngine;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuRacurs;

    private void Start()
    {
        panel.SetActive(false);
        menuPause.SetActive(true);
        menuRacurs.SetActive(false);
    }

    public void OpenPause()
    {
        panel.SetActive(true);
        menuPause.SetActive(true);
        menuRacurs.SetActive(false);

        GameManager.Instance.SetState(GameState.Pause);
        GameManager.Instance.PauseGame(true);
    }

    public void OpenRacursMenu()
    {
        menuPause.SetActive(false);
        menuRacurs.SetActive(true);
    }

    public void CloseRacursMenu()
    {
        menuPause.SetActive(true);
        menuRacurs.SetActive(false);
    }

    public void ContinueGame()
    {
        panel.SetActive(false);
        GameManager.Instance.SetState(GameState.GameScene);
        GameManager.Instance.PauseGame(false);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Возврат в главное меню");
        GameManager.Instance.PauseGame(false);
        GameManager.Instance.ResetSession(); 
        GameManager.Instance.SetState(GameState.MainMenu); 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
