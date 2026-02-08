using UnityEngine;

public class LoadMenuUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI mainMenu;
    public void LoadSlot(int slotId)
    {
        Debug.Log("Load save slot: " + slotId);
    }

    public void OnBack()
    {
        gameObject.SetActive(false);
        mainMenu.SetOverlayActive(false);
    }
}

