using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject lobbyPanel;

    [Header("Expedition")]
    [SerializeField]
    private string expeditionSceneName =
        "TestWorld";


    private void Start()
    {
        ShowMainMenu();
    }


    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(false);
        }
    }


    public void ShowLobby()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(true);
        }
    }


    public void StartExpedition()
    {
        SceneManager.LoadSceneAsync(
            expeditionSceneName
        );
    }


    public void QuitGame()
    {
        Application.Quit();

        Debug.Log(
            "QUIT requested. " +
            "Application.Quit only exits a built game."
        );
    }
}