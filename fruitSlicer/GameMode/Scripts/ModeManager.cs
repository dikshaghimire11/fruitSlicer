using UnityEngine;

public enum GameMode
{
    Infinite,
    JuiceMaking
}

public class ModeManager : MonoBehaviour
{
    private static ModeManager _instance;

    public static ModeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ModeManager>();

                if (_instance == null)
                {
                    GameObject managerGO = new GameObject("ModeManager_AutoCreated");
                    _instance = managerGO.AddComponent<ModeManager>();

                    DontDestroyOnLoad(managerGO);
                }
            }
            return _instance;
        }
    }

    public GameMode currentMode = GameMode.Infinite; // Default
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject); 
        }
    }
}