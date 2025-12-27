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

    // NEW: Flag to make sure we only show the message once
    private bool hasShownHighScoreMessage = false;

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        // Load High Score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        currentLives = maxLives;
        score = 0;
        isGameOver = false;

        // Reset the flag for the new game
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

        // --- NEW LOGIC: CHECK FOR HIGH SCORE ---
        // 1. Is the current score higher than the old high score?
        // 2. Is the old high score greater than 0? (Don't show on the very first game ever)
        // 3. Have we NOT shown the message yet this round?
        if (score > highScore && highScore > 0 && !hasShownHighScoreMessage)
        {
            hasShownHighScoreMessage = true; // Lock it so it doesn't appear again

            // Find the Blade script to use its floating text system
            Blade blade = FindObjectOfType<Blade>();
            if (blade != null)
            {
                // Show Green text in the center of the screen
                blade.ShowFloatingText("NEW HIGH SCORE!", Color.green, Vector3.zero);
            }
        }
    }

    // --- SHARED LIFE SYSTEM (Unchanged) ---
    public void LoseLife() { SubtractLife(); }
    public void HitBomb() { SubtractLife(); }

    private void SubtractLife()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateUI();

        if (currentLives <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        FruitSpawner.instance.HideFruitsLayer();
        int totalCoins=PlayerPrefs.GetInt("TotalCoins",100);
        PlayerPrefs.SetInt("TotalCoins",score+totalCoins);
        // Check for High Score (Save it permanently now)
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            if (victoryEffectPrefab != null)
            {
                Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FruitSpawner.instance.ShowFruitsLayer();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString("D4");

        // Update High Score text in real-time if we beat it
        int displayHighScore = (score > highScore) ? score : highScore;
        if (highScoreText != null) highScoreText.text = "HIGH: " + displayHighScore.ToString("D4");

        if (livesText != null) livesText.text = currentLives.ToString();
    }
}