using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartUI : MonoBehaviour
{
    public static RestartUI instance;  // Static instance for easy access


    public void LoadScene(string sceneName)
    {
      
        SceneManager.LoadScene(sceneName);
    }

    void Awake()
    {
        // Ensure the instance is properly set up
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Enforce singleton pattern
        }
    }

    public void OpenEndScreen()
    {
        if (PlayerManager.isGameOver)
        {
            PlayerManager.instance.gameOverScreen.SetActive(true);
            Time.timeScale = 0;  // Pause the game
            Debug.Log("Game Paused. Time.timeScale: " + Time.timeScale);
        }
        else
        {
            Debug.LogWarning("Game Over state not set or gameOverScreen is missing!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;  // Resume time for the restarted game
        PlayerManager.isGameOver = false;  // Reset game over state

        ReenableEnemyCollisions();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    private void ReenableEnemyCollisions()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            enemy.ReenableCollisions();
        }
    }
}
