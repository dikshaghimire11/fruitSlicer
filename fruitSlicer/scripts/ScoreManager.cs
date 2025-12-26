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

    // Flag to make sure we only show the message once
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
        hasShownHighScoreMessage = false;
        
        Time.timeScale = 1f; 
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // --- NEW: MODE CHECK ---
        // If we are in JUICE MODE, hide the Score texts. We only need Lives.
        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        }
        else
        {
            // Infinite Mode: Ensure they are visible
            if (scoreText != null) scoreText.gameObject.SetActive(true);
            if (highScoreText != null) highScoreText.gameObject.SetActive(true);
        }
        // -----------------------

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();

        // High Score Logic
        if (score > highScore && highScore > 0 && !hasShownHighScoreMessage)
        {
            hasShownHighScoreMessage = true; 

            Blade blade = FindObjectOfType<Blade>();
            if (blade != null)
            {
                blade.ShowFloatingText("NEW HIGH SCORE!", Color.green, Vector3.zero);
            }
        }
    }

    // --- SHARED LIFE SYSTEM ---
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

        // Only handle High Score if we are in Infinite Mode
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
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
        }

        if (finalScoreText != null)
        {
            // Custom Message based on Mode
            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                finalScoreText.text = "JUICE SPILLED!\nTRY AGAIN";
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
    }

    void UpdateUI()
    {
        // Only update score text if we are in Infinite Mode
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (scoreText != null) scoreText.text = score.ToString("D4");
            
            int displayHighScore = (score > highScore) ? score : highScore;
            if (highScoreText != null) highScoreText.text = "HIGH: " + displayHighScore.ToString("D4");
        }

        // Lives are updated in BOTH modes
        if (livesText != null) livesText.text = currentLives.ToString(); 
    }
}