using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner instance;

    // --- FRUIT SETTINGS ---
    // DRAG YOUR FRUIT PREFABS HERE IN THE INSPECTOR (Apple, Orange, etc.)
    public List<GameObject> fruitPrefabs; 

    public float spawnDelay = 2f;  
    public float spawnForce = 12f; 
    public GameObject gameContainer;

    // --- SPECIAL ITEMS SETTINGS ---
    [Header("Special Items Settings")]
    public GameObject bombPrefab;      
    public GameObject icePrefab;       
    public float specialSpawnDelay = 10f; 

    // --- INTERNAL VARIABLES ---
    private int lastFruitIndex = -1;    
    private int lastSpecialType = -1;
    private bool stopFruitSpawning = false; // Controls the "pause" during special events

    void Awake() 
    { 
        if (instance == null) instance = this; 
    }

    void Start()
    {
        StartCoroutine(SpawnFruitsRoutine());
        StartCoroutine(SpawnBombAndIceRoutine());
    }

    // --- 1. RANDOM FRUIT SPAWNING ROUTINE ---
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
             if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
              yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(spawnDelay);
            }
            

            // Only spawn if not paused by special event AND if we have fruits to spawn
            if (!stopFruitSpawning && fruitPrefabs.Count > 0)
            {
                // A. Pick a random fruit index (0 to List Size)
                int randomIndex = Random.Range(0, fruitPrefabs.Count);
                
                // B. Logic to prevent the exact same fruit twice in a row (makes it feel more random)
                if (fruitPrefabs.Count > 1)
                {
                    while (randomIndex == lastFruitIndex)
                    {
                        randomIndex = Random.Range(0, fruitPrefabs.Count);
                    }
                }
                
                lastFruitIndex = randomIndex;

                // C. Spawn the chosen fruit
                SpawnObject(fruitPrefabs[randomIndex], corners);
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
            // Wait for the delay
            yield return new WaitForSeconds(specialSpawnDelay);

            // Pause normal fruits
            stopFruitSpawning = true;
            yield return new WaitForSeconds(1.5f);

            GameObject prefabToSpawn = null;

            // --- MODE CHECK LOGIC ---
            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                // CAREER MODE: Always spawn Bomb (to punish cutting it)
                prefabToSpawn = bombPrefab; 
            }
            else 
            {
                // FREESTYLE MODE: Alternate between Ice and Bomb
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
            // ------------------------

            // Spawn the special item
            if (prefabToSpawn != null) SpawnObject(prefabToSpawn, corners);

            // Wait for it to clear, then resume normal fruits
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
        
        // Random X position within the container bounds
        float randomX = Random.Range(corners[0].x + upsetWidth, corners[3].x - upsetWidth);
        Vector3 spawnPosition = new Vector3(randomX, -3f, -10f); 

        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Launch Up
            rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
            
            // Add slight side force for randomness
            float randomHorizontalForce = Random.Range(-spawnForce / 2, spawnForce / 2);
            rb.AddForce(new Vector2(randomHorizontalForce, 0), ForceMode2D.Impulse);
        }
    }
}