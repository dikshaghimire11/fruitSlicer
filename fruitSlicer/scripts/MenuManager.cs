using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : MonoBehaviour
{
    // --- CAREER BUTTON (Juice Mode) ---
    public void careerMode()
    {
        ModeManager.Instance.currentMode = GameMode.JuiceMaking;
        
        // FIX: Matches the scene name in your Build Settings screenshot
        navigateToScene("UI"); 
    }

    // --- FREESTYLE BUTTON (Infinite Mode) ---
    public void freestyleMode()
    {
        ModeManager.Instance.currentMode = GameMode.Infinite;
        
        // FIX: Matches the scene name in your Build Settings screenshot
        navigateToScene("UI");
    }


    public void setting()
    {

    }

    public void highScore()
    {

    }

    public void shop()
    {
        navigateToScene("ShopScene");
    }

    public void mainMenu()
    {
        navigateToScene("MainMenuFruitSlicer");
    }

    public void navigateToScene(String sceneName)

    {
        TempData.sceneToLoad = sceneName;
        SceneManager.LoadSceneAsync("LoadingScene");
    }
}