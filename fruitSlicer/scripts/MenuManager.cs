using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class MenuManager : MonoBehaviour
{
    // [Header("Settings")]
    // public string sceneToLoad = "GameScene"; // Name of your actual game level

    // [Header("UI Elements")]
    // public Slider progressBar;
    // public TextMeshProUGUI progressText;

    // public GameObject loadingPanel;

    private void Start()
    {
        // Start the background loading process
        // StartCoroutine(LoadSceneAsync());
    }

    // IEnumerator LoadSceneAsync()
    // {
    //     // 1. Start loading the scene
    //     loadingPanel.SetActive(true);
    //     AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

    //     // Optional: If you want to wait for a "Press Any Key" at the end:
    //     // operation.allowSceneActivation = false;

    //     while (!operation.isDone)
    //     {
    //         // Unity's progress goes from 0 to 0.9. We normalize it to 0-1.
    //         float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

    //         // 2. Update the Slider
    //         if (progressBar != null)
    //             progressBar.value = progressValue;

    //         // 3. Update the Text
    //         if (progressText != null)
    //             progressText.text = "Loading " + (progressValue * 100).ToString("F0") + "%";

    //         // If using "allowSceneActivation = false", you'd check for input here
    //         // if (operation.progress >= 0.9f) { progressText.text = "Press Any Key"; ... }

    //         yield return null; // Wait until the next frame
    //     }
    // }

    public void careerMode()
    {

    }

    public void freestyleMode()
    {
        //  StartCoroutine(LoadSceneAsync());  
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

    public void navigateToScene(String sceneName)
    {
        TempData.sceneToLoad = sceneName;
        SceneManager.LoadSceneAsync("LoadingScene");
    }
}