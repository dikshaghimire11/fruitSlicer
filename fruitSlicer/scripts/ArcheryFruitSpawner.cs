using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcheryFruitSpawner : MonoBehaviour
{
    public static ArcheryFruitSpawner instance;

    [Header("Fruit Settings")]
    public List<GameObject> fruitPrefabs;
    public float spawnDelay = 2f;
    public float fallDuration = 4f;
    public AudioClip[] fruitsSliceSounds;

    [Header("Special Items Settings")]
    public GameObject bombPrefab;
    public float specialSpawnDelay = 10f;
    private int lastFruitIndex = -1;
    public GameObject gameContainer;
    private float randomDelay;
    private Camera mainCamera;
    public RectTransform spawnArea;

    void Awake()
    {
        if (instance == null)
            instance = this;
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
        StartCoroutine(SpawnFruitsRoutine(true));
        StartCoroutine(SpawnFruitsRoutine(false));
        StartCoroutine(SpawnBombRoutine());
    }
    IEnumerator SpawnFruitsRoutine(bool calculateDelay)
    {


        Vector3[] corners = new Vector3[4];
        if (gameContainer != null)
        {
            gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);
        }
        else
        {
            yield break;
        }

        while (true)
        {
            ;

            if (calculateDelay)
            {
                randomDelay = Random.Range(spawnDelay / 2f, spawnDelay);
            }
            yield return new WaitForSeconds(randomDelay);



            // 2. Spawn Logic
            if (fruitPrefabs.Count > 0)
            {

                GameObject prefabToSpawn = null;
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
                if (prefabToSpawn != null && Random.Range(0f, 1.1f) > 0.5f)
                {
                    SpawnObject(prefabToSpawn);
                }
            }
        }
    }
    // --- BOMB SPAWN ROUTINE ---
    IEnumerator SpawnBombRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(specialSpawnDelay);
            if (bombPrefab != null)
                SpawnObject(bombPrefab);
        }
    }

    // --- SPAWN OBJECT ---
    void SpawnObject(GameObject prefab)
    {
        if (prefab == null) return;

        // Get top of screen in world
        Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float horizontalMargin = 0.5f;

        float randomX = Random.Range(topLeft.x + horizontalMargin, topRight.x - horizontalMargin);

        float spawnY = topLeft.y + 1f; // little above screen

        Vector3 spawnPos = new Vector3(randomX, spawnY, -10f);

        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Bottom of screen
        float bottomY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        StartCoroutine(MoveDownWorld(newObj, fallDuration, bottomY));

        // Assign random slice sound
        Fruit fruitComp = newObj.GetComponent<Fruit>();

        if (!newObj.name.StartsWith("Coconut"))
        {
            newObj.transform.localScale *= 0.7f;

            if (fruitComp != null)
            {
                fruitComp.sliceSound = fruitsSliceSounds[Random.Range(0, fruitsSliceSounds.Length)];
            }
        }
        else
        {
            newObj.transform.localScale *= 0.7f;
        }
        Bomb bombComp = newObj.GetComponent<Bomb>();
        if (bombComp != null)
        {
            newObj.transform.localScale *= 0.8f; 

        }
    }
    IEnumerator MoveDownWorld(GameObject obj, float duration, float bottomY)
    {
        if (obj == null) yield break;

        Vector3 startPos = obj.transform.position;
        Vector3 endPos = new Vector3(startPos.x, bottomY - 1f, startPos.z);

        float elapsed = 0f;

        while (elapsed < duration && obj != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            obj.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
    }
}