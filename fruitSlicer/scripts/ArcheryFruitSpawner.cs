using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

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

    public RectTransform deSpawnArea;

    public float[] spawnPositions;

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
        StartCoroutine(SpawnFruitsRoutine());
        // StartCoroutine(SpawnFruitsRoutine(false));
        //StartCoroutine(SpawnBombRoutine());
    }
    IEnumerator SpawnFruitsRoutine()
    {


        UnityEngine.Vector3[] corners = new UnityEngine.Vector3[4];
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

            yield return new WaitForSeconds(spawnDelay);

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
                SpawnObject(prefabToSpawn);

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
        // Array to store the 4 corners
        UnityEngine.Vector3[] corners = new UnityEngine.Vector3[4];

        // Get the corners in world space
        spawnArea.GetWorldCorners(corners);

        // corners order: 0 = bottom-left, 1 = top-left, 2 = top-right, 3 = bottom-right
        // Get top of screen in world
        // Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        UnityEngine.Vector3 topRight = corners[2];
        UnityEngine.Vector3 bottomRight = corners[3];

        float horizontalMargin = 0.5f;

        float spawnX = topRight.x;

        float randomY;

        if (spawnPositions != null && spawnPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPositions.Length);
            randomY = spawnPositions[randomIndex];
        }
        else
        {
            randomY = Random.Range(topRight.y, bottomRight.y);
        }
        UnityEngine.Vector3 spawnPos = new UnityEngine.Vector3(spawnX, randomY, -10f);
        GameObject newObj = Instantiate(prefab, spawnPos, UnityEngine.Quaternion.identity);
        // Bottom of screen
        float rightX = deSpawnArea.transform.position.x;

        StartCoroutine(MoveDownWorld(newObj, fallDuration, rightX));

        // Assign random slice sound
        Fruit fruitComp = newObj.GetComponent<Fruit>();

        if (fruitComp != null)
        {
            if (!newObj.name.StartsWith("Coconut"))
            {
                newObj.transform.localScale *= fruitComp.uniformScale;
                fruitComp.sliceSound = fruitsSliceSounds[Random.Range(0, fruitsSliceSounds.Length)];

            }
            else
            {
                newObj.transform.localScale *= fruitComp.uniformScale;
            }
        }
        Bomb bombComp = newObj.GetComponent<Bomb>();
        if (bombComp != null)
        {
            bombComp.transform.localScale *= 0.8f;

        }
    }
    IEnumerator MoveDownWorld(GameObject obj, float duration, float rightX)
    {
        if (obj == null) yield break;

        UnityEngine.Vector3 startPos = obj.transform.position;
        UnityEngine.Vector3 endPos = new UnityEngine.Vector3(rightX, startPos.y, startPos.z);

        float elapsed = 0f;

        while (elapsed < duration && obj != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            obj.transform.position = UnityEngine.Vector3.Lerp(startPos, endPos, t);

            yield return null;
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
}