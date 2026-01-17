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

    public Sprite coinImage;

    public GameObject floatingReward;

    public Transform gameCanvas;


    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        alreadySavedCoins = 0;

        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayInfiniteMusic();
            }
        }
        // --- 1. AUTO-FIND PARENT LOGIC ---
        // If "ScoreParent" is empty, we find it automatically using the scoreText!
        if (scoreParent == null && scoreText != null)
        {
            scoreParent = scoreText.transform.parent.gameObject;
        }
        // ---------------------------------

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        currentLives = maxLives;
        score = 0;
        isGameOver = false;
        hasShownHighScoreMessage = false;

        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Apply the correct Mode Settings immediately
        UpdateUIStateBasedOnMode();
        UpdateTexts();
    }

    // --- 2. NEW FUNCTION: HIDE/SHOW PARENT BASED ON MODE ---
    void UpdateUIStateBasedOnMode()
    {
        if (ModeManager.Instance == null) return;

        bool isJuiceMode = (ModeManager.Instance.currentMode == GameMode.JuiceMaking);
        if (scoreParent != null)
        {
            if (isJuiceMode)
            {
                // 🍹 Juice Mode
                scoreParent.SetActive(false);
                if (x2ButtonForCareer != null)
                    x2ButtonForCareer.SetActive(false);
            }
            else
            {
                // ♾️ Infinite / Career Mode
                scoreParent.SetActive(true);    // Show score UI
                if (x2Button != null)
                    x2Button.SetActive(false);
            }
        }

    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;


        // High Score Logic (Infinite Mode Only)
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            score += amount;
            UpdateTexts();
            if (score > highScore && highScore > 0 && !hasShownHighScoreMessage)
            {
                hasShownHighScoreMessage = true;
                Blade blade = FindObjectOfType<Blade>();
                if (blade != null) blade.ShowFloatingText("NEW HIGH SCORE!", Color.green, Vector3.zero);
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayHighScoreSound();
                }
            }
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
        if (lifeBonusText != null) lifeBonusText.text = "=" + lifeBonus;
        if (pointsPerLevelText != null) pointsPerLevelText.text = "" + pointsPerLevel;
        if (bonusPointsText != null) bonusPointsText.text = "=" + bonus;
        finalPoints = finalPoints + bonus;
        // Add Points to Total Coins
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
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
        if (FruitSpawner.instance != null) FruitSpawner.instance.ShowFruitsLayer();
        isGameOver = false;
        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            JuiceManager.instance.isLevelActive = true;
            JuiceManager.instance.currentTime = JuiceManager.instance.currentTime + 10;
            GameObject floatingClockIcon = Instantiate(floatingReward, new Vector2(-0.5f, 0.0f), Quaternion.identity, gameCanvas);

            floatingClockIcon.GetComponentInChildren<Image>().sprite = clockImage;


            GameObject floatingLifeIcon = Instantiate(floatingReward, new Vector2(+0.5f, 0.0f), Quaternion.identity, gameCanvas);
            floatingLifeIcon.GetComponentInChildren<Image>().sprite = LifeImage;
            floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "1";


            StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
            StartCoroutine(moveFloatingRewardsAndDestroy(floatingClockIcon));
        }
        else
        {
            GameObject floatingLifeIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
            floatingLifeIcon.GetComponentInChildren<Image>().sprite = LifeImage;
            floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "1";
            StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
        }
        Time.timeScale = 1f;
        FruitSpawner.instance.startSpawnning();
    }

    public IEnumerator moveFloatingRewardsAndDestroy(GameObject rewardObject)
    {

        float timer = 0f;

        while (timer < 0.5f)
        {
            rewardObject.transform.Translate(Vector2.up * 3f * Time.deltaTime);
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
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            PlayerPrefs.SetInt("TotalCoins", totalCoins + JuiceManager.instance.finalPoints);
            if (missionPassEarnPoints != null) missionPassEarnPoints.text = "+" + JuiceManager.instance.finalPoints * 2;
            x2Button.SetActive(false);
        }
        else
        {
            if (finalScoreText != null)
            {

                finalScoreText.text = "SCORE: " + score * 2 + "\nHIGH SCORE: " + highScore;

            }
            PlayerPrefs.SetInt("TotalCoins", totalCoins + score);
            scoreText.text = (score * 2).ToString("D4");
            x2ButtonForCareer.SetActive(false);
            addLifeButton.SetActive(false);


        }
        GameObject floatingCoinIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
        floatingCoinIcon.GetComponentInChildren<Image>().sprite = coinImage;
        floatingCoinIcon.GetComponentInChildren<TextMeshProUGUI>().text = "x2";
        StartCoroutine(moveFloatingRewardsAndDestroy(floatingCoinIcon));



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
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();

        // Save Score/Coins


        // High Score Logic (Infinite Mode)
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {

            int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
            PlayerPrefs.SetInt("TotalCoins", score - alreadySavedCoins + totalCoins);
            alreadySavedCoins = score;
            PlayerPrefs.Save();
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

        }
        else
        {
            JuiceManager.instance.isLevelActive = false;
        }


        // Final Text Logic
        if (finalScoreText != null)
        {
            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                if (currentLives > 0) finalScoreText.text = "TIME'S UP!\nMISSION FAILED";
                else finalScoreText.text = "LIVES LOST!\nMISSION FAILED";
            }
            else
            {
                finalScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
            }
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayGameOverSound();
        }
    }

    public void RestartGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        AdsManager.instance.playInterestialAd();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (FruitSpawner.instance != null) FruitSpawner.instance.ShowFruitsLayer();

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
}