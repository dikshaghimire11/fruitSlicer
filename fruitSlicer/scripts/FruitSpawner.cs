using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner instance;
    // --- EXISTING VARIABLES ---
    public List<GameObject> fruitPrefabs;
    public float spawnDelay = 2f;  
    public float spawnForce = 12f; 
    public GameObject gameContainer;

    [Header("Special Items Settings")]
    public GameObject bombPrefab;      
    public GameObject icePrefab;       
    public float specialSpawnDelay = 10f; 

    // --- MEMORY VARIABLES ---
    private int lastFruitIndex = -1;    
    private int lastSpecialType = -1;
    
    // --- NEW: CONTROL FLAG ---
    private bool stopFruitSpawning = false; // "Traffic light" for fruits

    void Awake()
    {
        if (instance == null) { instance = this; }
    }
    void Start()
    {
        StartCoroutine(SpawnFruitsRoutine());
        StartCoroutine(SpawnBombAndIceRoutine());
    }

    // --- FRUIT ROUTINE (Listens to the traffic light) ---
    IEnumerator SpawnFruitsRoutine()
    {
        Vector3[] corners = new Vector3[4];
        gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);

        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            // ONLY spawn if the special routine says it's okay
            if (!stopFruitSpawning && fruitPrefabs.Count > 0)
            {
                int randomIndex = Random.Range(0, fruitPrefabs.Count);
                
                // Prevent duplicate fruits logic
                if (fruitPrefabs.Count > 1)
                {
                    while (randomIndex == lastFruitIndex)
                    {
                        randomIndex = Random.Range(0, fruitPrefabs.Count);
                    }
                }
                
                lastFruitIndex = randomIndex;
                SpawnObject(fruitPrefabs[randomIndex], corners);
            }
        }
    }

    // --- SPECIAL ITEM ROUTINE (Controls the traffic light) ---
    IEnumerator SpawnBombAndIceRoutine()
    {
        Vector3[] corners = new Vector3[4];
        gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);

        while (true)
        {
            // 1. Wait for the big gap (10 seconds)
            yield return new WaitForSeconds(specialSpawnDelay);

            // 2. PAUSE FRUITS! 
            // We turn the red light on so no fruits spawn while we prepare.
            stopFruitSpawning = true;

            // 3. Wait a tiny bit to let existing fruits clear out (optional, e.g. 1.5 seconds)
            yield return new WaitForSeconds(1.5f);

            // 4. Decide what to spawn (Alternating logic)
            GameObject prefabToSpawn = null;
            int currentType = 0;

            if (lastSpecialType == 0) 
            {
                currentType = 1;      
                prefabToSpawn = icePrefab;
            }
            else 
            {
                currentType = 0;      
                prefabToSpawn = bombPrefab;
            }
            lastSpecialType = currentType;

            // 5. Spawn the Special Item
            if (prefabToSpawn != null)
            {
                SpawnObject(prefabToSpawn, corners);
            }

            // 6. Wait a bit so the item is alone on screen (e.g. 2 seconds)
            yield return new WaitForSeconds(1f);

            // 7. RESUME FRUITS!
            stopFruitSpawning = false;
        }
    }

    // --- HELPER FUNCTION ---
    void SpawnObject(GameObject prefab, Vector3[] corners)
    {
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
}