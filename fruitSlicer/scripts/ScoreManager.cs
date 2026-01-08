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
    private GameObject scoreParent;         // <--- The Object holding Background + Text
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public TextMeshProUGUI missionPassEarnPoints;

    public GameObject x2Button;

    public GameObject addLifeButton;

    [Header("Victory Effect")]
    public GameObject victoryEffectPrefab;

    [Header("Game Rules")]
    public int maxLives = 3;

    private int score = 0;
    private int highScore = 0;
    private int currentLives;
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
        Debug.Log("Already Saved Coins:" + alreadySavedCoins);

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
                scoreParent.SetActive(false); // HIDE entire box in Juice Mode
            }
            else
            {
                scoreParent.SetActive(true);  // SHOW entire box in Infinite Mode
            }
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateTexts();

        // High Score Logic (Infinite Mode Only)
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (score > highScore && highScore > 0 && !hasShownHighScoreMessage)
            {
                hasShownHighScoreMessage = true;
                Blade blade = FindObjectOfType<Blade>();
                if (blade != null) blade.ShowFloatingText("NEW HIGH SCORE!", Color.green, Vector3.zero);
            }
        }
    }

    public void WinGame(int pointsPerLevel)
    {
        isGameOver = true;
        // Time.timeScale = 0f; 

        // if (gameOverPanel != null) gameOverPanel.SetActive(true);
        GameCanvasManager.instance.missionAccomplished();
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();
        if (missionPassEarnPoints != null) missionPassEarnPoints.text = "+" + pointsPerLevel;

        // Add Points to Total Coins
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        PlayerPrefs.SetInt("TotalCoins", totalCoins + pointsPerLevel);
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
        Debug.Log("Should Start");
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
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        PlayerPrefs.SetInt("TotalCoins", totalCoins + JuiceManager.instance.pointsPerLevel);
        if (missionPassEarnPoints != null) missionPassEarnPoints.text = "+" + JuiceManager.instance.pointsPerLevel * 2;
        x2Button.SetActive(false);
        GameObject floatingLifeIcon = Instantiate(floatingReward, Vector2.zero, Quaternion.identity, gameCanvas);
        floatingLifeIcon.GetComponentInChildren<Image>().sprite = coinImage;
        floatingLifeIcon.GetComponentInChildren<TextMeshProUGUI>().text = "x2";
        StartCoroutine(moveFloatingRewardsAndDestroy(floatingLifeIcon));
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
        Time.timeScale = 0f;


        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();

        // Save Score/Coins
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        PlayerPrefs.SetInt("TotalCoins", score - alreadySavedCoins + totalCoins);
        alreadySavedCoins = score;
        PlayerPrefs.Save();

        // High Score Logic (Infinite Mode)
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
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
            if (scoreText != null) scoreText.text = score.ToString("D4");

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