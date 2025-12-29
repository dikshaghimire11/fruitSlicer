using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

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
        Time.timeScale = 0f; 

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (FruitSpawner.instance != null) FruitSpawner.instance.HideFruitsLayer();
        if (finalScoreText != null) finalScoreText.text = "LEVEL COMPLETE!\nYOU WON!";

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
        UpdateTexts();
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
                if (currentLives > 0) finalScoreText.text = "TIME'S UP!\nMISSION FAILED";
                else finalScoreText.text = "LIVES LOST!\nMISSION FAILED";
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