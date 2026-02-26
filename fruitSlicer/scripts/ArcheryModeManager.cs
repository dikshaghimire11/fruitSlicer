using UnityEngine;

public class ArcheryModeManager : MonoBehaviour
{

    public float spawnDelay = 5f;

    public float specialSpawnDelay = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FruitSpawner fs = FruitSpawner.instance;
        fs.spawnDelay = spawnDelay;
        fs.specialSpawnDelay = specialSpawnDelay;

        ModeManager mm = ModeManager.Instance;
        mm.currentMode = GameMode.Archery;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
