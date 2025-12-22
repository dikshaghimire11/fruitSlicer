using UnityEngine;
using TMPro; // Essential for TextMesh Pro

public class ScoreManager : MonoBehaviour
{
    // The static instance allows other scripts to access the score easily
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText; // Drag your UI text here in the Inspector
    private int score = 0;

    void Awake()
    {
        // Singleton pattern: makes it easy for any script to find this one
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        UpdateUI();
    }

    // Call this function from other scripts to add points
    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text =score.ToString("D4");
    }
}