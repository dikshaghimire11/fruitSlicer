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

    public RectTransform deSpawnArea;

    public float[] spawnPositions;
    private int lastSpawnIndex = -1;

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
        if (fruitPrefabs == null || fruitPrefabs.Count == 0)
            yield break;

        GameObject specialFruit = fruitPrefabs[0];

        float nextSpecialTime = Time.time + 25f;

        while (true)
        {
            if (ScoreManager.instance.isGameOver)
                yield break;

            GameObject prefabToSpawn = null;

            if (Time.time >= nextSpecialTime)
            {
                prefabToSpawn = specialFruit;
                nextSpecialTime = Time.time + 25f;
            }
            else
            {
                List<GameObject> normalFruits = new List<GameObject>(fruitPrefabs);
                normalFruits.Remove(specialFruit);

                if (normalFruits.Count > 0)
                {
                    int randomIndex = Random.Range(0, normalFruits.Count);
                    prefabToSpawn = normalFruits[randomIndex];
                }
            }

            if (prefabToSpawn != null)
                SpawnObject(prefabToSpawn);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    // --- BOMB SPAWN ROUTINE ---
    // IEnumerator SpawnBombRoutine()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(specialSpawnDelay);
    //         if (bombPrefab != null)
    //             SpawnObject(bombPrefab);
    //     }
    // }
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
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, spawnPositions.Length);
            }
            while (randomIndex == lastSpawnIndex && spawnPositions.Length > 1);

            lastSpawnIndex = randomIndex;
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
        AudioSource audio = newObj.GetComponentInChildren<AudioSource>();
        if (audio != null)
        {
            audio.Stop();
        }
        if (fruitComp != null)
        {

            newObj.transform.localScale *= fruitComp.uniformScale;
            if (!newObj.name.StartsWith("Coconut"))
            {
                fruitComp.sliceSound = fruitsSliceSounds[Random.Range(0, fruitsSliceSounds.Length)];


            }
        }
        SpecialObject specialComp = newObj.GetComponent<SpecialObject>();
        if (specialComp != null)
        {
            newObj.transform.localScale *= 0.7f;
        }

        // Bomb bombComp = newObj.GetComponent<Bomb>();
        // if (bombComp != null)
        // {
        //     bombComp.transform.localScale *= 0.8f;

        // }
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
        {
                    mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Fruits"));
                    mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("FruitsDown"));  

        }
  

    }

    public void ShowFruitsLayer()
    {
        if (mainCamera != null)
            mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("Fruits"));
            mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("FruitsDown"));

    }
}