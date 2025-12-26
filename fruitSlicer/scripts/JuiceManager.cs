using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JuiceManager : MonoBehaviour
{
    public static JuiceManager instance;

    [Header("UI References")]
    public TextMeshProUGUI taskText;      // Shows "Cut: APPLE"
    public TextMeshProUGUI timerText;     // Shows "Time: 45" (Optional: Drag in if you have it)
    public TextMeshProUGUI counterText;   // Shows "Score: 3/10" (Optional)

    [Header("Level Settings")]
    public float levelDuration = 60f;     // Time limit in seconds
    public int requiredCuts = 10;         // How many correct fruits to win

    [Header("Game State")]
    public FruitType targetFruit;
    private float currentTime;
    private int currentCuts = 0;
    private bool isLevelActive = false;

    void Awake() 
    { 
        instance = this; 
    }

    void Start()
    {
        // Debugging: Check if ModeManager exists
        if (ModeManager.Instance == null)
        {
            Debug.LogError("CRITICAL: ModeManager is missing!");
            return;
        }

        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            Debug.Log("Starting TIMED JUICE MODE...");
            
            // Enable Text UIs if they are assigned
            if (taskText != null) taskText.gameObject.SetActive(true);
            if (timerText != null) timerText.gameObject.SetActive(true);
            if (counterText != null) counterText.gameObject.SetActive(true);

            StartLevel();
        }
        else
        {
            // Hide everything in Freestyle
            if (taskText != null) taskText.gameObject.SetActive(false);
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (counterText != null) counterText.gameObject.SetActive(false);
            gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        // Only run timer if the level is active
        if (isLevelActive)
        {
            currentTime -= Time.deltaTime;

            // Update Timer UI
            if (timerText != null)
            {
                timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
            }

            // Check for Time Out
            if (currentTime <= 0)
            {
                currentTime = 0;
                EndLevel(false); // FALSE means you lost
            }
        }
    }

    void StartLevel()
    {
        currentCuts = 0;
        currentTime = levelDuration;
        isLevelActive = true;
        
        UpdateCounterUI();
        PickNewTarget();
    }

    void PickNewTarget()
    {
        // Pick a random fruit (0 to 4)
        targetFruit = (FruitType)Random.Range(0, 5);
        
        if (taskText != null)
        {
            taskText.text = "CUT: " + targetFruit.ToString().ToUpper();
        }
    }

    public void CheckFruit(FruitType slicedType)
    {
        if (!isLevelActive) return;

        Debug.Log("You Sliced: " + slicedType + " | Target was: " + targetFruit);

        // 1. CORRECT FRUIT
        if (slicedType == targetFruit)
        {
            currentCuts++;
            UpdateCounterUI();

            // Check Win Condition
            if (currentCuts >= requiredCuts)
            {
                EndLevel(true); // TRUE means you won
            }
            else
            {
                // Keep playing: Pick a NEW target immediately
                PickNewTarget();
            }
        }
        // 2. WRONG FRUIT
        else
        {
            Debug.Log("WRONG CUT! Losing Life...");
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.LoseLife();
            }
        }
    }

    public void HitBomb()
    {
        if (!isLevelActive) return;
        Debug.Log("BOMB HIT in Juice Mode!");
        
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.LoseLife();
        }
    }

    void EndLevel(bool playerWon)
    {
        isLevelActive = false;

        if (playerWon)
        {
            Debug.Log("VICTORY! You collected " + requiredCuts + " fruits!");
            if (taskText != null) taskText.text = "VICTORY!";
            // Add any "Level Complete" UI logic here later
        }
        else
        {
            Debug.Log("GAME OVER! Time ran out.");
            if (taskText != null) taskText.text = "TIME'S UP!";
            if (ScoreManager.instance != null) ScoreManager.instance.LoseLife(); // Optional: Punish for time out
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