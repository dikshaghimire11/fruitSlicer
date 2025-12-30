using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic; // <--- REQUIRED for Enum.GetNames

public class JuiceManager : MonoBehaviour
{
    public static JuiceManager instance;

    [Header("UI References")]
    private GameObject timersParent;       // Auto-found
    public TextMeshProUGUI taskText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI counterText;


    public Image missionFruitIcon;

    [Header("Level Settings")]
    public float levelDuration = 60f;
    public int requiredCuts = 10;
    public int pointsPerLevel = 50;

    [Header("Game State")]
    // public FruitType targetFruit;
    public GameObject targetFruitNew;

    private float currentTime;
    private int currentCuts = 0;
    private bool isLevelActive = false;
    private int lastTimeInt = -1;

    public TextMeshProUGUI missionText;

    void Awake() { instance = this; }

    void Start()
    {

        // ... your existing code ...

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayCareerMusic();
        }

        if (ModeManager.Instance == null) return;

        // Auto-find logic
        if (timersParent == null && timerText != null)
        {
            timersParent = timerText.transform.parent.gameObject;
        }

        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            if (timersParent != null) timersParent.SetActive(true);
            if (taskText != null) taskText.gameObject.SetActive(true);
            if (counterText != null) counterText.gameObject.SetActive(true);
            if (missionFruitIcon != null) missionFruitIcon.gameObject.SetActive(true);

            StartLevel();
        }
        else
        {
            if (timersParent != null) timersParent.SetActive(false);
            if (taskText != null) taskText.gameObject.SetActive(false);
            if (counterText != null) counterText.gameObject.SetActive(false);

            if (missionFruitIcon != null) missionFruitIcon.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isLevelActive && GameCanvasManager.instance.startSpawning)
        {
            currentTime -= Time.deltaTime;

            int timeInt = Mathf.CeilToInt(currentTime);
            if (timeInt != lastTimeInt)
            {
                if (timerText != null) timerText.text = timeInt.ToString();
                lastTimeInt = timeInt;
            }

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

        PickNewTargetNew();

    }

    // --- FIX IS HERE ---
    // void PickNewTarget()
    // {
    //     // 1. Automatically count how many items are in the FruitType List
    //     // This will return '8' for your current list (Apple...Coconut)
    //     int fruitCount = Enum.GetNames(typeof(FruitType)).Length;

    //     // 2. Pick a random number between 0 and the Total Count
    //     targetFruit = (FruitType)UnityEngine.Random.Range(0, fruitCount);

    //     if (taskText != null)
    //     {
    //         taskText.text = targetFruit.ToString(); // Displays "Coconut", "Mango", etc.

    //         taskText.text = "" + targetFruit.ToString();
    //         missionText.text = "I want to have some fresh " + targetFruit.ToString() + " Juice...";

    //     }
    // }
    // -------------------

        void PickNewTargetNew()
    {
        List<GameObject> fruitsPrefab=FruitSpawner.instance.fruitPrefabs;
        // 1. Automatically count how many items are in the FruitType List
        // This will return '8' for your current list (Apple...Coconut)
        int fruitCount =  UnityEngine.Random.Range(0, fruitsPrefab.Count);
        Debug.Log("Count is: "+fruitCount);
        // 2. Pick a random number between 0 and the Total Count

        targetFruitNew = fruitsPrefab[fruitCount];

        if (taskText != null)
        {
    
            taskText.text = "" + targetFruitNew.name.ToString();
            missionText.text = "I want to have some fresh " + targetFruitNew.name.ToString() + " Juice...";
            missionFruitIcon.sprite=targetFruitNew.GetComponent<SpriteRenderer>().sprite;

        }
    }

    public void CheckFruit(String slicedFruitName)
    {
        if (!isLevelActive) return;
        Debug.Log(slicedFruitName+" - "+targetFruitNew.name+"(Clone)");
        if (slicedFruitName == targetFruitNew.name+"(Clone)")
        {
            currentCuts++;
            UpdateCounterUI();

            if (currentCuts >= requiredCuts)
            {
                EndLevel(true);
            }
        }
        else
        {
            if (ScoreManager.instance != null) ScoreManager.instance.LoseLife();
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
            if (ScoreManager.instance != null) ScoreManager.instance.ForceGameOver();
        }
    }

    void UpdateCounterUI()
    {
        if (counterText != null) counterText.text = currentCuts + " / " + requiredCuts;
    }
}