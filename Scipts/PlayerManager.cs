using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static bool isGameOver;
    public static bool isGameComplete; // New flag for game completion
    public static bool isPaused; // New flag for pausing the game
    public static PlayerManager instance;
    public GameObject gameCompleteScreen;
    public GameObject gameOverScreen;
    public GameObject PausePanel; // Reference to the Pause Panel UI
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Enforce singleton pattern
        }

        isGameOver = false;
        isGameComplete = false; // Initialize the game completion flag
        isPaused = false; // Initialize the pause state
        Time.timeScale = 1; // Ensure time is running when the game starts
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            HandleGameOver();
        }

        if (isGameComplete)
        {
            HandleGameComplete();
        }

        // Handle pause functionality
        if (Input.GetKeyDown(KeyCode.Escape)) // Example key to toggle pause (Escape key)
        {
            TogglePause();
        }
    }

    // Function to handle game over scenario
    private void HandleGameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        playerMovement.enabled = false; // Disable player movement when the game is over
    }

    // Function to handle game completion scenario
    private void HandleGameComplete()
    {
        gameCompleteScreen.SetActive(true);
        Time.timeScale = 0;
        playerMovement.enabled = false; // Disable player movement when the game is complete
    }

    // Function to toggle the game's paused state
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Function to pause the game
    public void PauseGame()
    {
        isPaused = true;
        PausePanel.SetActive(true); // Show the pause panel
        Time.timeScale = 0; // Stop the game's time
        playerMovement.enabled = false; // Disable player movement when paused
    }

    // Function to resume the game
    public void ResumeGame()
    {
        isPaused = false;
        PausePanel.SetActive(false); // Hide the pause panel
        Time.timeScale = 1; // Resume the game's time
        playerMovement.enabled = true; // Enable player movement
    }

    // Call this method when the game is completed
    public void CompleteGame()
    {
        isGameComplete = true;
    }

    // Call this method when the game is over
    public void EndGame()
    {
        isGameOver = true;
    }
}
