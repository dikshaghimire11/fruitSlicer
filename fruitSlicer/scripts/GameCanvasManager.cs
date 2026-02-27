using UnityEngine;
using UnityEngine.UI; // Required for UI
using System.Collections;
using System;
using NUnit.Framework;
using Random = UnityEngine.Random;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameCanvasManager : MonoBehaviour
{
    // public RectTransform imageToMove; // Drag your Image here
    // public Vector2 targetPosition;    // Set X and Y in Inspector (e.g., 0, 0 for center)
    // public float duration = 2.0f;     // Duration in seconds

    // Call this function to start the movement (e.g., via a Button)

    public static GameCanvasManager instance;

    public bool startSpawning;
    public GameObject shopGameObject;
    public GameObject choppingBoard;

    public GameObject workDesk;

    private GameObject newObject;

    public GameObject missionPanel;

    public GameObject freeModeCharacter;
    public GameObject archeryCharacter;

    public GameObject missionAccomplishedPanel;

    public GameObject juiceContainer;

    public Image juiceColor;

    public GameObject addLifeOnlyObject;

    public GameObject addLifeAndTimeObject;

    public GameObject hideUIPausePanel;
    public GameObject tutorialPanel;

    public VideoPlayer tutorialVideoPlayer;
    public GameObject closeButton;
    public GameObject goToShopPanel;


    public GameObject reviewPanel;
    private bool waitingForReview;

    public GameObject notBlurredShopBackground;
    public GameObject bowPrefab;
    private GameObject bowInstance;







    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        bool navigateToShop = PlayerPrefs.GetInt("NavigateToShop", 0) == 1;
        bool hasSeenTutorial = PlayerPrefs.GetInt("HasSeenTutorial", 0) == 1;
        bool hasReviewedMission = PlayerPrefs.GetInt("HasReview", 0) == 1;
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", PlayerConfig.defaultCoins);

        if (!navigateToShop && hasSeenTutorial)
        {
            showGoToShopPanel();

        }
        else if (navigateToShop && !hasReviewedMission && totalCoins > (2000))
        {
            showGoToReviewPanel();
        }

        else
        {
            startGame();

            // StartMoving();

        }

    }
    public void missionAccepted()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        if (ModeManager.Instance.currentMode == GameMode.Archery)
        {
            GameObject bow = Instantiate(bowPrefab, bowPrefab.transform.position, bowPrefab.transform.rotation, this.transform);

        }

        notBlurredShopBackground.SetActive(false);
        missionPanel.SetActive(false);
        if (SoundManager.instance != null) SoundManager.instance.PlayCharacterGoneSound();
        bool hasSeenTutorial = PlayerPrefs.GetInt("HasSeenTutorial", 0) == 1;
        if (!hasSeenTutorial)
        {
            PlayTutorialVideo();
        }
        else
        {

            unHideUiInteraction();

            startSpawnAction();

            StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 2, newObject));
            StartCoroutine(MoveRoutine(new Vector2(0f, -4f), 1, 2, workDesk));

        }

        // choppingBoard.SetActive(true);

    }

    public void startSpawnAction()
    {
        startSpawning = true;
        switch (ModeManager.Instance.currentMode)
        {

            case GameMode.Archery:


                ArcheryFruitSpawner.instance.startSpawnning();
                break;
            case GameMode.JuiceMaking:
            case GameMode.Infinite:
                FruitSpawner.instance.startSpawnning();
                break;
        }
    }
    // public void StartMoving()
    // {
    //     // StartCoroutine(MoveRoutine(targetPosition, duration));
    // }

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
            missionPanel.SetActive(true);
            hideUIInterction();
            // shopGameObject.SetActive(false);
        }
        else if (type == 3)
        {
            goToShopPanel.SetActive(true);
        }
        else if (type == 4)
        {
            reviewPanel.SetActive(true);
        }


    }

    public void missionAccomplished()
    {
        if (missionAccomplishedPanel != null)
        {
            Color colorofJuice = JuiceManager.instance.targetFruitNew.GetComponent<Fruit>().juiceColor;
            colorofJuice.a = 100;
            juiceColor.color = colorofJuice;
            if (SoundManager.instance != null) SoundManager.instance.PlayCharacterPopSound();
            StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 0, newObject));
            StartCoroutine(MoveRoutine(new Vector2(0, 0f), 1, 0, workDesk));
            StartCoroutine(MoveRoutine(new Vector2(-0.2f, -1.7f), 1, 0, juiceContainer));

            missionAccomplishedPanel.SetActive(true);

            hideUIInterction();
        }
    }

    public void nextOrderAfterMissionAccomplished()
    {
        if (missionAccomplishedPanel != null)
        {

            missionAccomplishedPanel.SetActive(false);
            unHideUiInteraction();
            ScoreManager.instance.RestartGame();
        }
    }

    public void disableAllErrorMessages()
    {
        GameObject[] errorMessages = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in errorMessages)
        {
            if (obj.tag == "ErrorMessage")
            {
                GameObject.Destroy(obj);
            }
        }
    }

    public void getMoreLife()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        AdsManager.instance.currentAdvertisement = AdsManager.AdvertisementType_.AddLife;

        AdsManager.instance.onClickAddLife();
    }

    public void get2xReward()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        AdsManager.instance.currentAdvertisement = AdsManager.AdvertisementType_.x2Reward;
        AdsManager.instance.onClickAddLife();
    }

    public void hideUIInterction()
    {
        if (hideUIPausePanel != null)
        {
            hideUIPausePanel.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
        GameObject bladeInstance = GameObject.FindGameObjectWithTag("Blade");
        if (bladeInstance != null)
        {
            TrailRenderer trailRenderer = bladeInstance.GetComponent<TrailRenderer>();
            trailRenderer.enabled = false;
        }
        bowInstance = GameObject.FindGameObjectWithTag("Bow");
        if (bowInstance != null)
        {
            bowInstance.SetActive(false);
        }

    }

    public void unHideUiInteraction()
    {
        if (hideUIPausePanel != null)
        {
            hideUIPausePanel.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        GameObject bladeInstance = GameObject.FindGameObjectWithTag("Blade");
        if (bladeInstance != null)
        {
            TrailRenderer trailRenderer = bladeInstance.GetComponent<TrailRenderer>();
            trailRenderer.enabled = true;
        }
        if (bowInstance != null)
        {
            bowInstance.SetActive(true);
        }
    }
    public void PlayTutorialVideo()
    {
        tutorialPanel.SetActive(true);
        tutorialVideoPlayer.time = 0;
        tutorialVideoPlayer.Play();
    }
    public void CloseTutorial()
    {
        tutorialVideoPlayer.Stop();
        tutorialPanel.SetActive(false);

        PlayerPrefs.SetInt("HasSeenTutorial", 1);
        PlayerPrefs.Save();
        unHideUiInteraction();

        startSpawnAction();

        StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 2, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0f, -4f), 1, 2, workDesk));
    }
    public void showGoToShopPanel()
    {
        if (newObject != null)
        {
            Destroy(newObject);
        }
        newObject = Instantiate(freeModeCharacter, freeModeCharacter.transform.position, freeModeCharacter.transform.rotation, shopGameObject.transform);
        newObject.transform.SetSiblingIndex(0);

        StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 3, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0, 0f), 1, 3, workDesk));
    }
    public void goToShop()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        goToShopPanel.SetActive(false);
        PlayerPrefs.SetInt("NavigateToShop", 1);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("ShopScene");

    }
    public void showGoToReviewPanel()
    {
        if (newObject != null)
        {
            Destroy(newObject);
        }
        newObject = Instantiate(freeModeCharacter, freeModeCharacter.transform.position, freeModeCharacter.transform.rotation, shopGameObject.transform);
        newObject.transform.SetSiblingIndex(0);

        StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 4, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0, 0f), 1, 4, workDesk));
    }


    public void goToPlayStore()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        reviewPanel.SetActive(false);
        PlayerPrefs.SetInt("HasReview", 1);
        PlayerPrefs.Save();
        waitingForReview = true;
        OpenPlayStoreReview();

    }

    public void OpenPlayStoreReview()
    {
#if UNITY_ANDROID
    string packageName = Application.identifier;

    // Open Play Store app directly to the review page
    string reviewUrl = "market://details?id=" + packageName + "&reviewId=0";
    string fallbackUrl = "https://play.google.com/store/apps/details?id=" + packageName;

    try
    {
        Application.OpenURL(reviewUrl);
    }
    catch
    {
        Application.OpenURL(fallbackUrl); 
    }
#else
        // Non-Android platforms: open web page
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#endif
    }
    private void OnApplicationPause(bool pause)
    {
        if (!pause && waitingForReview)
        {
            if (newObject != null)
            {
                Destroy(newObject);
            }
            startGame();
            waitingForReview = false;
        }
    }
    public void closeReviewPanel()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        if (newObject != null)
        {
            Destroy(newObject);
        }
        PlayerPrefs.SetInt("HasReview", 1);
        PlayerPrefs.Save();
        reviewPanel.SetActive(false);
        startGame();
    }

    public void startGame()
    {
        GameObject character = null;
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Archery:
                character = archeryCharacter;
                addLifeOnlyObject.SetActive(true);
                break;
            case GameMode.JuiceMaking:
                int length = ShopLists.instance.characterList.Length;
                character = ShopLists.instance.characterList[Random.Range(0, length)];
                choppingBoard.SetActive(false);
                addLifeAndTimeObject.SetActive(true);
                break;
            case GameMode.Infinite:
                character = freeModeCharacter;
                addLifeOnlyObject.SetActive(true);
                break;
        }
        newObject = Instantiate(character, character.transform.position, character.transform.rotation, shopGameObject.transform);
        newObject.transform.SetSiblingIndex(0);
        if (SoundManager.instance != null) SoundManager.instance.PlayCharacterPopSound();


        StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 1, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0, 0f), 1, 0, workDesk));
    }

}