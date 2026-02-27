using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI References")]
    private GameObject scoreParent;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI livesText;

    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public TextMeshProUGUI missionPassEarnPoints;
    public TextMeshProUGUI timeBonusText;
    public TextMeshProUGUI lifeBonusText;
    public TextMeshProUGUI pointsPerLevelText;
    public TextMeshProUGUI bonusPointsText;

    public TextMeshProUGUI remainingLives;

    public TextMeshProUGUI remainingTime;



    public GameObject x2Button;
    public GameObject x2ButtonForCareer;

    public GameObject addLifeButton;

    [Header("Victory Effect")]
    public GameObject victoryEffectPrefab;

    [Header("Game Rules")]
    public int maxLives = 3;

    private int score = 0;
    private int bonus = 0;
    private int highScore = 0;

    public int currentLives;
    public bool isGameOver = false;
    private bool hasShownHighScoreMessage = false;

    private int alreadySavedCoins;

    public Sprite clockImage;
    public Sprite LifeImage;
    public Sprite bonusImage;

    public Sprite coinImage;

    public GameObject floatingReward;

    public Transform gameCanvas;
    public TextMeshProUGUI freezeCountdownText;
    private GameObject timerParent;
    public Button specialRewardButton;

    private int sliceCount = 1;

    public TextMeshProUGUI specialRewardText;

    public Sprite archeryArrowttackSprite;



    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        alreadySavedCoins = 0;

        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayInfiniteMusic();
                }
                break;
            case GameMode.JuiceMaking:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayCareerMusic();
                }
                break;
            case GameMode.Archery:
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayArcheryMusic();
                }
                break;
        }


        // --- 1. AUTO-FIND PARENT LOGIC ---
        // If "ScoreParent" is empty, we find it automatically using the scoreText!
        if (scoreParent == null && scoreText != null)
        {
            scoreParent = scoreText.transform.parent.gameObject;
        }
        // ---------------------------------
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
                highScore = PlayerPrefs.GetInt("HighScore", 0);
                break;
            case GameMode.Archery:
                highScore = PlayerPrefs.GetInt("HighScoreForArchery", 0);
                break;
        }


        currentLives = maxLives;
        score = 0;
        isGameOver = false;
        hasShownHighScoreMessage = false;

        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GameCanvasManager.instance.unHideUiInteraction();

        // Apply the correct Mode Settings immediately
        UpdateUIStateBasedOnMode();
        UpdateTexts();
    }

    // --- 2. NEW FUNCTION: HIDE/SHOW PARENT BASED ON MODE ---
    void UpdateUIStateBasedOnMode()
    {
        if (ModeManager.Instance == null) return;
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
                scoreParent.SetActive(true);    // Show score UI
                if (x2Button != null)
                    x2Button.SetActive(false);
                break;
            case GameMode.JuiceMaking:
                scoreParent.SetActive(false);
                if (x2ButtonForCareer != null)
                    x2ButtonForCareer.SetActive(false);
                break;
            default:
                // Default to showing score for safety
                if (scoreParent != null) scoreParent.SetActive(true);
                break;
            case GameMode.Archery:
                scoreParent.SetActive(true);    // Show score UI
                if (x2Button != null)
                    x2Button.SetActive(false);
                break;
        }


    }
    public void AddScore(int amount)
    {
        if (isGameOver) return;
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
            case GameMode.Archery:
                score += amount;
                UpdateTexts();
                bool isFirstTime = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0;

                if (!isFirstTime && score > highScore && !hasShownHighScoreMessage)
                {
                    hasShownHighScoreMessage = true;

                    if (SoundManager.instance != null)
                    {
                        SoundManager.instance.PlayHighScoreSound();
                    }
                    Blade blade = FindObjectOfType<Blade>();

                    if (blade != null)
                    {
                        Vector3 worldPos = Camera.main.transform.position + Camera.main.transform.forward * 5f;

                        blade.ShowFloatingText(
                            "NEW HIGH SCORE!",
                            Color.green,
                            worldPos,
                            1.5f,
                            0.5f
                        );
                    }

                    Arrow arrow = FindObjectOfType<Arrow>();
                    if (arrow != null)
                    {
                        Vector3 worldPos = Camera.main.transform.position + Camera.main.transform.forward * 5f;

                        arrow.ShowFloatingText(
                            "NEW HIGH SCORE!",
                            Color.green,
                            worldPos,
                            1.5f,
                            0.5f
                        );
                    }
                }
                break;
        }
    }

    public void addBonusAmount(int amount)
    {
        bonus += amount;
    }

    public void WinGame(int pointsPerLevel, int timeBonus, int lifeBonus, int finalPoints)
    {
        isGameOver = true;
        // Time.timeScale = 0f; 

        // if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayGameWinSound();
        }
        GameCanvasManager.instance.missionAccomplished();
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();
        if (missionPassEarnPoints != null) missionPassEarnPoints.text = "TOTAL=" + (finalPoints + bonus);
        if (timeBonusText != null) timeBonusText.text = "=" + timeBonus;
        if (remainingLives != null) remainingLives.text = "+" + instance.currentLives;
        if (remainingTime != null) remainingTime.text = "+" + Mathf.CeilToInt(JuiceManager.instance.currentTime);
        if (lifeBonusText != null) lifeBonusText.text = "=" + lifeBonus;
        if (pointsPerLevelText != null) pointsPerLevelText.text = "" + pointsPerLevel;
        if (bonusPointsText != null) bonusPointsText.text = "=" + bonus;
        finalPoints = finalPoints + bonus;
        // Add Points to Total Coins
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", PlayerConfig.defaultCoins);
        PlayerPrefs.SetInt("TotalCoins", totalCoins + finalPoints);
        PlayerPrefs.Save();

        if (victoryEffectPrefab != null) Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);

    }

    public void ForceGameOver()
    {
        currentLives = 0;
        UpdateTexts();
        EndGame();
    }

    public void LoseLife() { SubtractLife(); }
    public void HitBomb() { SubtractLife(); }

    private void SubtractLife()
    {
        if (isGameOver) return;
        currentLives--;
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayLifeLostSound();
        }
        UpdateTexts();
        if (currentLives <= 0) EndGame();
    }

    public void addLifeAndResumeGame()
    {

        addLifeButton.SetActive(false);
        destroyAllSpawnnedObjects();
        currentLives++;
        if (livesText != null) livesText.text = currentLives.ToString();
        gameOverPanel.SetActive(false);
        GameCanvasManager.instance.unHideUiInteraction();
        if (FruitSpawner.instance != null) FruitSpawner.instance.ShowFruitsLayer();
        if (ArcheryFruitSpawner.instance != null) ArcheryFruitSpawner.instance.ShowFruitsLayer();
        isGameOver = false;
        if (specialRewardButton != null && sliceCount > 0)
        {
            specialRewardButton.gameObject.SetActive(true);
            specialRewardText.text = "" + sliceCount;
        }
        GameObject floatingLifeIcon;
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
            case GameMode.Archery:
                floatingLifeIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
                floatingLifeIcon.GetComponentInChildren<Image>().sprite = LifeImage;
                floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "1";
                StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
                break;
            case GameMode.JuiceMaking:
                JuiceManager.instance.isLevelActive = true;
                JuiceManager.instance.currentTime = JuiceManager.instance.currentTime + 10;
                GameObject floatingClockIcon = Instantiate(floatingReward, new Vector2(-0.5f, 0.0f), Quaternion.identity, gameCanvas);

                floatingClockIcon.GetComponentInChildren<Image>().sprite = clockImage;


                floatingLifeIcon = Instantiate(floatingReward, new Vector2(+0.5f, 0.0f), Quaternion.identity, gameCanvas);
                floatingLifeIcon.GetComponentInChildren<Image>().sprite = LifeImage;
                floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "1";


                StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
                StartCoroutine(moveFloatingRewardsAndDestroy(floatingClockIcon));
                break;
        }
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayLiveAndBonusAddedSound();
        }
        Time.timeScale = 1f;
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

    public IEnumerator moveFloatingRewardsAndDestroy(GameObject rewardObject)
    {

        float timer = 0f;

        while (timer < 1f)
        {
            rewardObject.transform.Translate(Vector2.up * 1.5f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        GameObject.Destroy(rewardObject);
    }


    public void x2Reward()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", PlayerConfig.defaultCoins);
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
            case GameMode.Archery:
                if (finalScoreText != null)
                {

                    finalScoreText.text = "SCORE: " + score * 2 + "\nHIGH SCORE: " + highScore;

                }
                PlayerPrefs.SetInt("TotalCoins", totalCoins + score);
                scoreText.text = (score * 2).ToString("D4");
                x2ButtonForCareer.SetActive(false);
                addLifeButton.SetActive(false);
                break;
            case GameMode.JuiceMaking:
                PlayerPrefs.SetInt("TotalCoins", totalCoins + JuiceManager.instance.finalPoints);
                if (missionPassEarnPoints != null) missionPassEarnPoints.text = "+" + JuiceManager.instance.finalPoints * 2;
                x2Button.SetActive(false);
                break;
        }
        GameObject floatingCoinIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
        floatingCoinIcon.GetComponentInChildren<Image>().sprite = coinImage;
        floatingCoinIcon.GetComponentInChildren<TextMeshProUGUI>().text = "x2";
        StartCoroutine(moveFloatingRewardsAndDestroy(floatingCoinIcon));
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayLiveAndBonusAddedSound();
        }


    }

    private void destroyAllSpawnnedObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == 6)
            {
                GameObject.Destroy(obj);
            }
        }
    }
    void EndGame()
    {
        isGameOver = true;



        // Time.timeScale = 0f;


        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        GameCanvasManager.instance.hideUIInterction();
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();
        if (ArcheryFruitSpawner.instance != null) ArcheryFruitSpawner.instance.HideFruitsLayer();

        // Save Score/Coins


        // High Score Logic (Infinite Mode)
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", PlayerConfig.defaultCoins);
        switch
        (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:

                PlayerPrefs.SetInt("TotalCoins", score - alreadySavedCoins + totalCoins);
                alreadySavedCoins = score;
                PlayerPrefs.Save();
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HighScore", highScore);
                    PlayerPrefs.Save();
                }
                PlayerPrefs.SetInt("HasPlayedBefore", 1);
                PlayerPrefs.Save();

                break;
            case GameMode.JuiceMaking:
                JuiceManager.instance.isLevelActive = false;
                break;
            case GameMode.Archery:

                PlayerPrefs.SetInt("TotalCoins", score - alreadySavedCoins + totalCoins);
                alreadySavedCoins = score;
                PlayerPrefs.Save();
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HighScoreForArchery", highScore);
                    PlayerPrefs.Save();
                }
                PlayerPrefs.SetInt("HasPlayedBefore", 1);
                PlayerPrefs.Save();
                if (specialRewardButton != null)
                    specialRewardButton.gameObject.SetActive(false);
                break;
        }
        ;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayGameOverSound();
        }
        // Final Text Logic
        if (finalScoreText != null)
        {
            switch (ModeManager.Instance.currentMode)
            {
                case GameMode.Infinite:
                case GameMode.Archery:
                    finalScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
                    break;
                case GameMode.JuiceMaking:
                    if (currentLives > 0) finalScoreText.text = "MISSION FAILED!\nTIME'S UP!";
                    else finalScoreText.text = "MISSION FAILED!\nLIVES LOST!";
                    break;

            }


        }
    }

    public void ShowShopPanelAfterDelay()
    {

        if (GameCanvasManager.instance != null)
        {
            GameCanvasManager.instance.showGoToShopPanel();
            if (GameCanvasManager.instance.missionAccomplishedPanel != null)
            {
                GameCanvasManager.instance.missionAccomplishedPanel.SetActive(false);
            }
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
    }

    public void RestartGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        bool navigateToShop = PlayerPrefs.GetInt("NavigateToShop", 0) == 1;
        if (!navigateToShop)
        {
            ShowShopPanelAfterDelay();

        }
        else
        {
            AdsManager.instance.playInterestialAd();

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if (FruitSpawner.instance != null) FruitSpawner.instance.ShowFruitsLayer();
            if (ArcheryFruitSpawner.instance != null) ArcheryFruitSpawner.instance.ShowFruitsLayer();
        }

    }

    // --- 3. SIMPLIFIED UPDATE TEXTS ---
    // This now only updates numbers. Visibility is handled in Start().
    void UpdateTexts()
    {
        // Only try to update score text if the parent is active/exists
        if (scoreParent != null && scoreParent.activeSelf)
        {
            if (scoreText != null) scoreText.text = (score).ToString("D4");

            if (highScoreText != null)
            {
                int displayHighScore = (score > highScore) ? score : highScore;
                highScoreText.text = "HIGH: " + displayHighScore.ToString("D4");
            }
        }

        // Lives are always visible in both modes
        if (livesText != null) livesText.text = currentLives.ToString();
    }
    public IEnumerator showTimer(float freezeDuration) // ✅ IEnumerator
    {
        if (timerParent == null && freezeCountdownText != null)
        {
            timerParent = freezeCountdownText.transform.parent.gameObject;
        }

        if (timerParent != null)
            timerParent.SetActive(true);

        float timer = freezeDuration;
        freezeCountdownText.gameObject.SetActive(true);

        while (timer > 0)
        {
            freezeCountdownText.text = Mathf.Ceil(timer).ToString();
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        freezeCountdownText.gameObject.SetActive(false);
        timerParent.SetActive(false);
    }

    public void addSpecialReward()
    {
        if (specialRewardButton != null)
        {
            specialRewardButton.gameObject.SetActive(true);
        }
        sliceCount += 1;
        if (specialRewardText != null)
        {
            specialRewardText.text = "" + sliceCount;
        }
        GameObject floatingLifeIcon;
        floatingLifeIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
        floatingLifeIcon.GetComponentInChildren<Image>().sprite = archeryArrowttackSprite;
        floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "1";
        StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
    }

    public void reduceArrowAttackCount()
    {
        sliceCount -= 1;
        if (specialRewardText != null)
        {
            specialRewardText.text = "" + sliceCount;
        }
        if (sliceCount <= 0)
        {
            specialRewardButton.gameObject.SetActive(false);
        }
    }


}