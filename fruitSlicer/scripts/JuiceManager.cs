using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JuiceManager : MonoBehaviour
{
    public static JuiceManager instance;

    [Header("UI References")]
    public TextMeshProUGUI taskText;      
    public TextMeshProUGUI timerText;     
    public TextMeshProUGUI counterText;   

    [Header("Level Settings")]
    public float levelDuration = 60f;     
    public int requiredCuts = 10;     
    public int pointsPerLevel = 50;    

    [Header("Game State")]
    public FruitType targetFruit;
    private float currentTime;
    private int currentCuts = 0;
    private bool isLevelActive = false;

    void Awake() { instance = this; }

    void Start()
    {
        if (ModeManager.Instance == null) return;

        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            if (taskText != null) taskText.gameObject.SetActive(true);
            if (timerText != null) timerText.gameObject.SetActive(true);
            if (counterText != null) counterText.gameObject.SetActive(true);

            StartLevel();
        }
        else
        {
            if (taskText != null) taskText.gameObject.SetActive(false);
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (counterText != null) counterText.gameObject.SetActive(false);
            gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        if (isLevelActive)
        {
            currentTime -= Time.deltaTime;
            if (timerText != null) timerText.text = "" + Mathf.CeilToInt(currentTime).ToString();

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndLevel(false); 
            }
        }
    }

    void StartLevel()
    {
        currentCuts = 0;
        currentTime = levelDuration;
        isLevelActive = true;
        
        UpdateCounterUI();
        
        // --- THIS IS CALLED ONLY ONCE NOW ---
        PickNewTarget(); 
    }

    void PickNewTarget()
    {
        targetFruit = (FruitType)Random.Range(0, 5); // Picks ONE fruit for the whole level
        
        if (taskText != null)
        {
            taskText.text = "" + targetFruit.ToString();
        }
    }

    public void CheckFruit(FruitType slicedType)
    {
        if (!isLevelActive) return;

        // 1. CORRECT FRUIT
        if (slicedType == targetFruit)
        {
            currentCuts++;
            UpdateCounterUI();

            // --- CHANGE HERE: We do NOT change the target anymore! ---
            // The player must keep cutting the SAME fruit until they reach 10.

            if (currentCuts >= requiredCuts)
            {
                EndLevel(true); 
            }
        }
        // 2. WRONG FRUIT
        else
        {
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.LoseLife();
            }
        }
    }

    public void HitBomb()
    {
        if (!isLevelActive) return;
        if (ScoreManager.instance != null) ScoreManager.instance.LoseLife();
    }

    void EndLevel(bool playerWon)
    {
        isLevelActive = false;

        if (playerWon)
        {
            if (taskText != null) taskText.text = "VICTORY!";
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(pointsPerLevel); 
                ScoreManager.instance.WinGame(pointsPerLevel);    
            }
        }
        else
        {
            if (taskText != null) taskText.text = "TIME'S UP!";
            
            // Call the NEW Instant Game Over function
            if (ScoreManager.instance != null) 
            {
                ScoreManager.instance.ForceGameOver(); 
            }
            // ----------------------
        }
    }

    void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = currentCuts + " / " + requiredCuts;
        }
    }
}