
using UnityEngine;

public class HideUIPanel : MonoBehaviour
{
    public GameObject gameOverPanel;

    public GameObject missionPanel;

    public GameObject missionCompletePanel;

    public GameObject pausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(gameOverPanel.activeSelf  || missionPanel.activeSelf || missionCompletePanel.activeSelf || pausePanel.activeSelf)
        {
            gameObject.GetComponent<UnityEngine.UI.Image>().enabled=true;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Image>().enabled=false;
        }
    }
}
