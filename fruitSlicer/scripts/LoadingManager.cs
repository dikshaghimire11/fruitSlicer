using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    void Awake()
    {
        // Safety check: If we just hit Play in this scene, go back to Main Menu
        if (string.IsNullOrEmpty(TempData.sceneToLoad))
        {
            TempData.sceneToLoad = "MainMenuFruitSlicer";
        }
        
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 1. Log what we are trying to load
        Debug.Log("Attempting to load scene: " + TempData.sceneToLoad);

        // 2. Start loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(TempData.sceneToLoad);

        // --- ANTI-FREEZE FIX ---
        // If the scene is not in Build Settings, 'operation' will be null.
        // We must check this to prevent the game from freezing/crashing.
        if (operation == null)
        {
            Debug.LogError("CRITICAL ERROR: Scene '" + TempData.sceneToLoad + "' not found!");
            Debug.LogError("Solution: Go to File > Build Settings and ensure '" + TempData.sceneToLoad + "' is in the list and CHECKED.");
            if (progressText != null) progressText.text = "Error: Scene Not Found";
            yield break; // Stop here safely
        }
        // -----------------------

        while (!operation.isDone)
        {
            // Unity progress is 0.0 to 0.9. Normalize to 0.0 to 1.0
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progressValue;

            if (progressText != null)
                progressText.text = "Loading " + (progressValue * 100).ToString("F0") + "%";

            yield return null; 
        }
    }
}