using UnityEngine;
using TMPro; // Required for TextMesh Pro
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
    public int maxLives = 3; // Total lives shared between Bombs and Misses

    private int score = 0;
    private int highScore = 0;
    private int currentLives;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        // Load High Score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Reset Game State
        currentLives = maxLives;
        score = 0;
        isGameOver = false;
        
        Time.timeScale = 1f; 
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();
    }

    // --- SHARED LIFE SYSTEM ---
    
    // 1. Called when Fruit falls (Fruit.cs)
    public void LoseLife()
    {
        SubtractLife();
    }

    // 2. Called when Bomb is hit (Bomb.cs)
    public void HitBomb()
    {
        SubtractLife();
    }

    // This handles the actual math for BOTH events
    private void SubtractLife()
    {
        if (isGameOver) return;

        currentLives--; // Reduce life by 1
        
        UpdateUI(); 

        // If lives reach 0, Game Over
        if (currentLives <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Freeze Game
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Check for High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            // Play Victory Effect (Confetti)
            if (victoryEffectPrefab != null)
            {
                Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        // Show Final Score
        if (finalScoreText != null)
        {
            finalScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString("D4");
        
        if (highScoreText != null) highScoreText.text = "HIGH: " + highScore.ToString("D4");

        if (livesText != null) 
        {
            livesText.text = currentLives.ToString(); 
        }
    }
}