using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Ensure time is resumed before switching scenes
        Time.timeScale = 1;  // Always reset time scale before switching scenes
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Exits the game (only works in a built application, not in the editor)
        Debug.Log("Game is quitting...");  // Logs to console, useful for testing in the editor
        Application.Quit();
    }

    void Start()
    {
        // Reset time scale when returning to the main menu
        Time.timeScale = 1;
    }

    void Update()
    {
        
    }
}
