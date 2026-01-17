using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : MonoBehaviour
{
    // --- CAREER BUTTON (Juice Mode) ---
    public void careerMode()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        ModeManager.Instance.currentMode = GameMode.JuiceMaking;

        // FIX: Matches the scene name in your Build Settings screenshot
        navigateToScene("UI");
    }

    // --- FREESTYLE BUTTON (Infinite Mode) ---
    public void freestyleMode()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        ModeManager.Instance.currentMode = GameMode.Infinite;

        // FIX: Matches the scene name in your Build Settings screenshot
        navigateToScene("UI");
    }


    public void setting()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        navigateToScene("SettingScene");
    }

    public void highScore()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        navigateToScene("HighScoreScene");
    }

    public void shop()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        navigateToScene("ShopScene");
    }

    public void mainMenu()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        navigateToScene("MainMenuFruitSlicer");
    }

    public void navigateToScene(String sceneName)

    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        TempData.sceneToLoad = sceneName;
        SceneManager.LoadSceneAsync("LoadingScene");
    }
    void Start()
    {
        // ... your existing code ...

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMenuMusic();
        }
    }
}