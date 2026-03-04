using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcheryFruitSpawner : MonoBehaviour
{
    public static ArcheryFruitSpawner instance;

    [Header("Fruit Settings")]
    public List<GameObject> fruitPrefabs;
    public AudioClip[] fruitsSliceSounds;

    // [Header("Special Items Settings")]
    // public GameObject bombPrefab;
    public RectTransform spawnArea;
    public RectTransform deSpawnArea;
    public float[] spawnPositions;
    private int fruitThresholdForHint = 8;

    [Header("Level Configuration")]
    public List<ArcheryWaveDTO> levels;

    private ArcheryWaveDTO currentLevelData;
    private int lastSpawnIndex = -1;

    private Camera mainCamera;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        int playerLevel = PlayerPrefs.GetInt("ArcheryPlayerLevel", 1);
        SetLevel(playerLevel);
    }

    public void startSpawnning()
    {
        StartCoroutine(SpawnFruitsRoutine());
        // StartCoroutine(SpawnFruitsRoutine(false));
        //StartCoroutine(SpawnBombRoutine());
    }

    IEnumerator SpawnFruitsRoutine()
    {
        if (fruitPrefabs == null || fruitPrefabs.Count == 0 || currentLevelData == null)
        {
            yield break;
        }

        GameObject specialFruit = fruitPrefabs[0];
        float nextSpecialTime = Time.time + currentLevelData.specialFruitInterval;

        int spawnedCount = 0;

        while (spawnedCount < currentLevelData.totalFruits)
        {
            if (ScoreManager.instance.isGameOver) yield break;

            GameObject prefabToSpawn = null;

            // Spawn special fruit if interval reached
            if (Time.time >= nextSpecialTime && specialFruit != null)
            {
                prefabToSpawn = specialFruit;
                nextSpecialTime = Time.time + currentLevelData.specialFruitInterval;
            }
            else
            {
                // Normal fruits
                List<GameObject> normalFruits = new List<GameObject>(fruitPrefabs);
                if (specialFruit != null) normalFruits.Remove(specialFruit);

                if (normalFruits.Count > 0)
                    prefabToSpawn = normalFruits[Random.Range(0, normalFruits.Count)];
                else
                    prefabToSpawn = fruitPrefabs[0]; // fallback if only 1 fruit exists
            }

            if (prefabToSpawn != null)
            {
                SpawnObject(prefabToSpawn);
                spawnedCount++;
            }

            CheckFruitCount();

            yield return new WaitForSeconds(currentLevelData.spawnDelay);
        }
    }

    void SpawnObject(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3[] corners = new Vector3[4];
        spawnArea.GetWorldCorners(corners);
        Vector3 topRight = corners[2];
        Vector3 bottomRight = corners[3];

        float spawnX = topRight.x;
        float randomY = spawnPositions != null && spawnPositions.Length > 0 ?
                        spawnPositions[Random.Range(0, spawnPositions.Length)] :
                        Random.Range(topRight.y, bottomRight.y);

        Vector3 spawnPos = new Vector3(spawnX, randomY, -10f);
        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        float distance = Mathf.Abs(spawnPos.x - deSpawnArea.position.x);
        float moveDuration = currentLevelData.fruitSpeed > 0 ? distance / currentLevelData.fruitSpeed : 4f;

        StartCoroutine(MoveDownWorld(newObj, moveDuration, deSpawnArea.position.x));

        // Assign slice sound
        Fruit fruitComp = newObj.GetComponent<Fruit>();
        AudioSource audio = newObj.GetComponentInChildren<AudioSource>();
        if (audio != null) audio.Stop();

        if (fruitComp != null)
        {
            newObj.transform.localScale *= fruitComp.uniformScale;
            if (!newObj.name.StartsWith("Coconut"))
                fruitComp.sliceSound = fruitsSliceSounds[Random.Range(0, fruitsSliceSounds.Length)];
        }

        SpecialObject specialComp = newObj.GetComponent<SpecialObject>();
        if (specialComp != null) newObj.transform.localScale *= 0.7f;
    }

    IEnumerator MoveDownWorld(GameObject obj, float duration, float rightX)
    {
        if (obj == null) yield break;

        Vector3 startPos = obj.transform.position;
        Vector3 endPos = new Vector3(rightX, startPos.y, startPos.z);
        float elapsed = 0f;

        while (elapsed < duration && obj != null)
        {
            elapsed += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }
    }

    void CheckFruitCount()
    {
        int fruitCount = GameObject.FindGameObjectsWithTag("Fruits").Length;
        if (fruitCount >= fruitThresholdForHint)
        {
            ScoreManager.instance?.HighlightSpecialRewardButton();
        }
    }

    public void SetLevel(int playerLevel)
    {
        if (levels == null || levels.Count == 0) return;

        int index = Mathf.Clamp(playerLevel - 1, 0, levels.Count - 1);
        currentLevelData = levels[index];

        fruitThresholdForHint = currentLevelData.fruitThresholdForHint;

       // Debug.Log($"Loaded Level {playerLevel} | Total Fruits: {currentLevelData.totalFruits} | Speed: {currentLevelData.fruitSpeed}");
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
        {
            mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("Fruits"));
            mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("FruitsDown"));
        }
    }
}