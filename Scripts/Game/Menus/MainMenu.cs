using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame(bool playSolo = false)
    {
        GameManager.Instance.StartTournament(playSolo);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}