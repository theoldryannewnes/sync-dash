using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

}
