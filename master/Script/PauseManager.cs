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
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        pauseMenuPanel.SetActive(true);
        FruitSpawner.instance.HideFruitsLayer(); // Show the UI
        Time.timeScale = 0f;            // Stop game time
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        pauseMenuPanel.SetActive(false);
        FruitSpawner.instance.ShowFruitsLayer(); // Hide the UI
        Time.timeScale = 1f;             // Resume game time
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        Time.timeScale = 1f;
        TempData.sceneToLoad = "MainMenuFruitSlicer";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}