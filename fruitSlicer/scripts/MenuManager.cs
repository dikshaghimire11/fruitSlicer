using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    public GameObject noticePanel;
    public GameObject noticeCharacter;
    public GameObject noticeMessage;

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

        bool hasShownNotice = PlayerPrefs.GetInt("ShownNotice", 0) == 1;

        if (!hasShownNotice)
        {
            StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 1, noticeCharacter));
            noticePanel.SetActive(true);

        }

    }
    public void CloseNoticePanel()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 2, noticeCharacter));
        noticePanel.SetActive(false);
        PlayerPrefs.SetInt("ShownNotice", 1);
        PlayerPrefs.Save();
    }

    private IEnumerator MoveRoutine(Vector2 target, float time, int type, GameObject targetObject)
    // 1 is for enter the screen, 2 is for exit the screen
    {
        RectTransform rectTransform = targetObject.GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Calculate how far along we are (0.0 to 1.0)
            float t = elapsedTime / time;

            // Optional: Make it "Smooth" (Ease In and Out) instead of robotic linear speed
            // If you want constant speed, remove this line.
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            // Move the image
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, target, t);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it lands exactly on the target at the end
        rectTransform.anchoredPosition = target;
        if (type == 1)
        {
            noticeMessage.SetActive(true);
        }
        else if (type == 2)
        {
            noticeMessage.SetActive(false);
        }



    }
}