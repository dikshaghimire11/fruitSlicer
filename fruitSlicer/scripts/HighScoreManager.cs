using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTextForFreestyle;
    public TextMeshProUGUI scoreTextForCareer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreTextForFreestyle.text = "" + PlayerPrefs.GetInt("HighScore", 0).ToString();
        scoreTextForCareer.text = "" + PlayerPrefs.GetInt("HighScoreCareer", 0).ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
