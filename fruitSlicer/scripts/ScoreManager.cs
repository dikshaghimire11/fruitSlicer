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
        // Load High Score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        currentLives = maxLives;
        score = 0;
        isGameOver = false;
        hasShownHighScoreMessage = false;

        Time.timeScale = 1f; 
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // --- KEY CHANGE: HIDE SCORE IN JUICE MODE ---
        // We check the ModeManager. If it's JuiceMaking, we turn off the text objects.
        if (ModeManager.Instance != null && ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        }
        else
        {
            // Infinite Mode: Make sure they are visible
            if (scoreText != null) scoreText.gameObject.SetActive(true);
            if (highScoreText != null) highScoreText.gameObject.SetActive(true);
        }
        // ---------------------------------------------

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();

        // High Score Logic (Only show floating text in Infinite Mode)
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

    // --- WIN FUNCTION (Called by JuiceManager) ---
    public void WinGame()
    {
        isGameOver = true;
        Time.timeScale = 0f; 
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Show "Level Complete" message
        if (finalScoreText != null)
        {
            finalScoreText.text = "LEVEL COMPLETE!\nYOU WON!";
        }

        if (victoryEffectPrefab != null)
        {
            Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);
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

    // --- LOSE FUNCTION ---
    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        FruitSpawner.instance.HideFruitsLayer();
        int totalCoins=PlayerPrefs.GetInt("TotalCoins",100);
        PlayerPrefs.SetInt("TotalCoins",score+totalCoins);
        // Check for High Score (Save it permanently now)
        // 1. Infinite Mode Logic (Save High Score)
        if (ModeManager.Instance != null && ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
        }

        // 2. Set Final Text based on Mode
        if (finalScoreText != null)
        {
            if (ModeManager.Instance != null && ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                finalScoreText.text = "MISSION FAILED\nTRY AGAIN";
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
        // Only update Score Text if we are in Infinite Mode
        if (ModeManager.Instance != null && ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            if (scoreText != null) scoreText.text = score.ToString("D4");
            
            int displayHighScore = (score > highScore) ? score : highScore;
            if (highScoreText != null) highScoreText.text = "HIGH: " + displayHighScore.ToString("D4");
        }

        // Lives are always visible in both modes
        if (livesText != null) livesText.text = currentLives.ToString(); 
    }
}