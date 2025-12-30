using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner instance;

    // --- FRUIT SETTINGS ---

    [Header("Fruit Settings")]
    public List<GameObject> fruitPrefabs;
    public float spawnDelay = 2f;
    public float spawnForce = 12f;
    public GameObject gameContainer;

    // --- JUICE MODE SETTINGS (NEW) ---
    [Header("Juice Mode Settings")]
    [Range(0f, 1f)]
    public float targetFruitChance = 0.6f; // 60% chance to spawn the TARGET fruit

    // --- SPECIAL ITEMS SETTINGS ---
    [Header("Special Items Settings")]
    public GameObject bombPrefab;
    public GameObject icePrefab;
    public float specialSpawnDelay = 10f;

    // --- INTERNAL VARIABLES ---
    private int lastFruitIndex = -1;
    private int lastSpecialType = -1;

    private bool stopFruitSpawning = false;
    private Camera mainCamera;


    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

    }

    public void startSpawnning()
    {
        StartCoroutine(SpawnFruitsRoutine());
        StartCoroutine(SpawnBombAndIceRoutine());
    }

    // --- 1. FRUIT SPAWNING ROUTINE ---
    IEnumerator SpawnFruitsRoutine()
    {

        Vector3[] corners = new Vector3[4];
        if (gameContainer != null)
        {
            gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);
        }
        else
        {
            Debug.LogError("Game Container is missing! Assign it in the Inspector.");
            yield break;
        }

        while (true)
        {
            if (ScoreManager.instance.isGameOver)
            {
                Debug.Log("Entered");
                yield return new WaitForSeconds(1);
            }
                  Debug.Log("Out");
            // 1. Handle Delay based on Mode
            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                yield return new WaitForSeconds(1f); // Faster spawns in Juice Mode?

            }
            else
            {
                yield return new WaitForSeconds(spawnDelay);
            }


            // 2. Spawn Logic
            if (!stopFruitSpawning && fruitPrefabs.Count > 0)
            {

                GameObject prefabToSpawn = null;

                // // A. Pick a random fruit index (0 to List Size)
                // int randomIndex = Random.Range(0, fruitPrefabs.Count);

                // // B. Logic to prevent the exact same fruit twice in a row (makes it feel more random)
                // if (fruitPrefabs.Count > 1)
                // {
                //     while (randomIndex == lastFruitIndex)
                //     {
                //         randomIndex = Random.Range(0, fruitPrefabs.Count);
                //     }
                // }

                // lastFruitIndex = randomIndex;


                // --- NEW: SPAWN BIAS LOGIC ---
                // Check if we are in Juice Mode
                bool isJuiceMode = (ModeManager.Instance != null && ModeManager.Instance.currentMode == GameMode.JuiceMaking);

                // If Juice Mode, roll the dice to see if we force the Target Fruit
                if (isJuiceMode && Random.value < targetFruitChance && JuiceManager.instance != null)
                {
                    // Use your helper function to get the target prefab
                    prefabToSpawn = JuiceManager.instance.targetFruitNew;
                }

                // --- FALLBACK: RANDOM SPAWN ---
                // If we didn't force a target (or we are in Infinite mode), pick randomly
                if (prefabToSpawn == null)
                {
                    int randomIndex = Random.Range(0, fruitPrefabs.Count);

                    // Prevent same fruit twice in a row (Visual variety)
                    if (fruitPrefabs.Count > 1)
                    {
                        while (randomIndex == lastFruitIndex)
                        {
                            randomIndex = Random.Range(0, fruitPrefabs.Count);
                        }
                    }
                    lastFruitIndex = randomIndex;
                    prefabToSpawn = fruitPrefabs[randomIndex];
                }

                // 3. Execute Spawn
                if (prefabToSpawn != null)
                {
                    SpawnObject(prefabToSpawn, corners);
                }
            }
        }
    }

    // --- 2. SPECIAL ITEM (BOMB/ICE) ROUTINE ---
    IEnumerator SpawnBombAndIceRoutine()
    {

        Vector3[] corners = new Vector3[4];
        if (gameContainer != null) gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);

        while (true)
        {
            if (ScoreManager.instance.isGameOver)
            {
                yield return new WaitForSeconds(1);
            }
            yield return new WaitForSeconds(specialSpawnDelay);

            stopFruitSpawning = true;
            yield return new WaitForSeconds(1f);

            GameObject prefabToSpawn = null;

            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {

                prefabToSpawn = bombPrefab;

            }
            else
            {

                if (lastSpecialType == 0) { prefabToSpawn = icePrefab; lastSpecialType = 1; }
                else { prefabToSpawn = bombPrefab; lastSpecialType = 0; }

                if (lastSpecialType == 0)
                {
                    prefabToSpawn = icePrefab;
                    lastSpecialType = 1;
                }
                else
                {
                    prefabToSpawn = bombPrefab;
                    lastSpecialType = 0;
                }

            }

            if (prefabToSpawn != null) SpawnObject(prefabToSpawn, corners);

            yield return new WaitForSeconds(1f);
            stopFruitSpawning = false;
        }
    }

    // --- 3. HELPER FUNCTION TO LAUNCH OBJECTS ---
    void SpawnObject(GameObject prefab, Vector3[] corners)
    {
        if (prefab == null) return;

        float screenWidth = corners[3].x - corners[0].x;

        float upsetWidth = screenWidth * 0.2f;

        float randomX = Random.Range(corners[0].x + upsetWidth, corners[3].x - upsetWidth);
        Vector3 spawnPosition = new Vector3(randomX, -3f, -10f);

        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
            float randomHorizontalForce = Random.Range(-spawnForce / 2, spawnForce / 2);
            rb.AddForce(new Vector2(randomHorizontalForce, 0), ForceMode2D.Impulse);
        }
    }

    public void HideFruitsLayer()
    {

        if (mainCamera != null)
            mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Fruits"));

    }

    public void ShowFruitsLayer()
    {
        if (mainCamera != null)
            mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("Fruits"));
    }

    // Your existing helper function
    // public GameObject GetFruitsOfType(FruitType type)
    // {
    //     foreach (GameObject fruitPrefab in fruitPrefabs)
    //     {
    //         Fruit fruitComponent = fruitPrefab.GetComponent<Fruit>();
    //         if (fruitComponent != null && fruitComponent.fruitType == type)
    //         {
    //             return fruitPrefab;
    //         }
    //     }
    //     return null; 
    // }
}