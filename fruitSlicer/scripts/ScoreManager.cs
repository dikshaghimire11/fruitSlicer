using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Victory Effect")]
    public GameObject victoryEffectPrefab;

    [Header("Game Rules")]
    public int maxLives = 3;

    private int score = 0;
    private int highScore = 0;
    private int currentLives;
    private bool isGameOver = false;
    private bool hasShownHighScoreMessage = false;

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {


        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currentLives = maxLives;
        score = 0;
        isGameOver = false;
        hasShownHighScoreMessage = false;

        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();

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
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();
        if (finalScoreText != null) finalScoreText.text = "LEVEL COMPLETE!\nYOU WON!";

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        PlayerPrefs.SetInt("TotalCoins", totalCoins + pointsPerLevel);
        PlayerPrefs.Save();

        if (victoryEffectPrefab != null) Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);
    }

    // --- NEW: FORCE GAME OVER (For Timer) ---
    public void ForceGameOver()
    {
        currentLives = 0;
        UpdateUI();
        EndGame();
    }
    // ----------------------------------------

    public void LoseLife() { SubtractLife(); }
    public void HitBomb() { SubtractLife(); }

    private void SubtractLife()
    {
        if (isGameOver) return;
        currentLives--;
        UpdateUI();
        if (currentLives <= 0) EndGame();
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();

        // Save Score/Coins
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        PlayerPrefs.SetInt("TotalCoins", score + totalCoins);
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
                // Logic: If lives are 0, you died. If lives > 0, Time ran out.
                if (currentLives > 0)
                    finalScoreText.text = "TIME'S UP!\nMISSION FAILED";
                else
                    finalScoreText.text = "LIVES LOST!\nMISSION FAILED";
            }
            else
            {
                finalScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (FruitSpawner.instance != null) FruitSpawner.instance.ShowFruitsLayer();
    }

    // --- UPDATED UI LOGIC (Fixes the persistent Score issue) ---
    void UpdateUI()
    {
        bool isJuiceMode = (ModeManager.Instance.currentMode == GameMode.JuiceMaking);

        if (isJuiceMode)
        {
            // FORCE HIDE in Juice Mode
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        }
        else
        {
            // FORCE SHOW in Infinite Mode
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
                scoreText.text = score.ToString("D4");
            }
            if (highScoreText != null)
            {
                highScoreText.gameObject.SetActive(true);
                int displayHighScore = (score > highScore) ? score : highScore;
                highScoreText.text = "HIGH: " + displayHighScore.ToString("D4");
            }
        }

        // Lives are always visible
        if (livesText != null) livesText.text = currentLives.ToString();
    }
}