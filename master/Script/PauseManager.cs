using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Drag your PauseMenu panel here
    private bool isPaused = false;

    void Update()
    {
        // Toggle pause when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {Debug.Log("Pressed Pause");
        pauseMenuPanel.SetActive(true); // Show the UI
        Time.timeScale = 0f;            // Stop game time
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Hide the UI
        Time.timeScale = 1f;             // Resume game time
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANT: Reset time before switching scenes
        SceneManager.LoadScene("MainMenu"); // Replace with your scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}