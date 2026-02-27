using UnityEngine;

public class ArcheryModeManager : MonoBehaviour
{


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
}
