using UnityEngine;

public class ArcheryModeManager : MonoBehaviour
{

    public GameObject arrowAttackPrefab;
    public Transform gameCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ModeManager mm = ModeManager.Instance;
        mm.currentMode = GameMode.Archery;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void consumeSpecialReward()
    {
        ScoreManager.instance.reduceArrowAttackCount();
        Instantiate(arrowAttackPrefab, arrowAttackPrefab.transform.position, arrowAttackPrefab.transform.rotation, gameCanvas);

    }
}
