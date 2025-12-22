using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    // --- EXISTING VARIABLES ---
    public List<GameObject> fruitPrefabs;
    public float spawnDelay = 2f;  
    public float spawnForce = 10f; 
    public float minX = -2f;       
    public float maxX = 2f;
    public GameObject gameContainer;

    [Header("Special Items Settings")]
    public GameObject bombPrefab;      // Drag Bomb here
    public GameObject icePrefab;       // Drag Ice here
    public float specialSpawnDelay = 10f; // Gap between special items

    void Start()
    {
        // 1. Start the normal fruit loop (Fast)
        StartCoroutine(SpawnFruitsRoutine());

        // 2. Start the separate Bomb/Ice loop (Slow - 10 sec gap)
        StartCoroutine(SpawnBombAndIceRoutine());
    }

    // --- EXISTING FRUIT ROUTINE (Unchanged logic) ---
    IEnumerator SpawnFruitsRoutine()
    {
        Vector3[] corners = new Vector3[4];
        gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);

        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (fruitPrefabs.Count > 0)
            {
                // Logic for Fruits
                int randomIndex = Random.Range(0, fruitPrefabs.Count);
                GameObject randomFruitPrefab = fruitPrefabs[randomIndex];
                SpawnObject(randomFruitPrefab, corners); // I moved the spawn logic to a helper function to avoid repeating code
            }
        }
    }

    // --- NEW ROUTINE: HANDLES BOMB & ICE ONLY ---
    IEnumerator SpawnBombAndIceRoutine()
    {
        Vector3[] corners = new Vector3[4];
        gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);

        while (true)
        {
            // Wait for 10 seconds (or whatever you set in Inspector)
            yield return new WaitForSeconds(specialSpawnDelay);

            // Randomly pick Bomb (0) or Ice (1)
            GameObject prefabToSpawn = null;
            if (Random.Range(0, 2) == 0) 
            {
                prefabToSpawn = bombPrefab;
            }
            else 
            {
                prefabToSpawn = icePrefab;
            }

            // Spawn the special item
            if (prefabToSpawn != null)
            {
                SpawnObject(prefabToSpawn, corners);
            }
        }
    }

    // --- HELPER FUNCTION ---
    // This keeps your original spawning math exactly the same, 
    // but lets us use it for both Fruits and Bombs/Ice.
    void SpawnObject(GameObject prefab, Vector3[] corners)
    {
        float screenWidth = corners[3].x - corners[0].x;
        float upsetWidth = screenWidth * 0.2f; 
        
        float randomX = Random.Range(corners[0].x + upsetWidth, corners[3].x - upsetWidth);
        Vector3 spawnPosition = new Vector3(randomX, -3f, -10f); // Spawns from bottom

        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
            
            float randomHorizontalForce = Random.Range(-spawnForce / 2, spawnForce / 2);
            rb.AddForce(new Vector2(randomHorizontalForce, 0), ForceMode2D.Impulse);
        }
        Destroy(newObj, 5f);
    }
}