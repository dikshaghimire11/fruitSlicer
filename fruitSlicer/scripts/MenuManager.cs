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

    public GameObject modeAddedNoticePanel;
    public GameObject modeAddedNoticeCharacter;
    public GameObject modeAddedNoticeMessage;

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

    public void archeryMode()
    {
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayButtonClickSound();
            }
            ModeManager.Instance.currentMode = GameMode.Archery;

            // FIX: Matches the scene name in your Build Settings screenshot
            navigateToScene("Archery");
        }
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

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMenuMusic();
        }

        bool hasShownNotice = PlayerPrefs.GetInt("ShownNotice", 0) == 1;
        bool hasShownModeAddedNotice = PlayerPrefs.GetInt("ShownModeAddedNotice", 0) == 1;

        if (!hasShownNotice)
        {
            StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 1, noticeCharacter));
            noticePanel.SetActive(true);

        }
        if (hasShownNotice && !hasShownModeAddedNotice)
        {
            StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 3, modeAddedNoticeCharacter));
            modeAddedNoticePanel.SetActive(true);
        }

    }
    public void CloseModeAddedNoticePanel()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 4, modeAddedNoticeCharacter));
        modeAddedNoticePanel.SetActive(false);
        PlayerPrefs.SetInt("ShownModeAddedNotice", 1);
        PlayerPrefs.Save();
    }

    public void tryNowArcheryMode()
    {
        PlayerPrefs.SetInt("ShownModeAddedNotice", 1);
        PlayerPrefs.Save();
        archeryMode();
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
    public void tryNowButton()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 2, noticeCharacter));
        noticePanel.SetActive(false);
        PlayerPrefs.SetInt("ShownNotice", 1);
        PlayerPrefs.Save();
        archeryMode();
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
        else if (type == 3)
        {
            modeAddedNoticeMessage.SetActive(true);
        }
        else if (type == 4)
        {
            modeAddedNoticeMessage.SetActive(false);
        }



    }
}