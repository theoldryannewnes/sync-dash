using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void ShowMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

}
