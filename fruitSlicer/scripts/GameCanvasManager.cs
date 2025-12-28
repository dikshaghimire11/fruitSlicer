using UnityEngine;
using UnityEngine.UI; // Required for UI
using System.Collections;
using System;
using NUnit.Framework;
using Random = UnityEngine.Random;

public class GameCanvasManager : MonoBehaviour
{
    // public RectTransform imageToMove; // Drag your Image here
    // public Vector2 targetPosition;    // Set X and Y in Inspector (e.g., 0, 0 for center)
    // public float duration = 2.0f;     // Duration in seconds

    // Call this function to start the movement (e.g., via a Button)

    public static GameCanvasManager instance;

    public bool startSpawning;
    public GameObject shopGameObject;
    public GameObject choppingBoard;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        // StartMoving();

        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            int length = ShopLists.instance.characterList.Length;
            choppingBoard.SetActive(false);
            GameObject character = ShopLists.instance.characterList[Random.Range(0, length)];
            GameObject newObject = Instantiate(character, character.transform.position, character.transform.rotation, shopGameObject.transform);
            newObject.transform.SetSiblingIndex(1);
        }
        else
        {
            missionAccepted();
        }

    }

    public void missionAccepted()
    {
        startSpawning = true;
        shopGameObject.SetActive(false);
        choppingBoard.SetActive(true);
        FruitSpawner.instance.startSpawnning();
    }
    // public void StartMoving()
    // {
    //     // StartCoroutine(MoveRoutine(targetPosition, duration));
    // }

    // private IEnumerator MoveRoutine(Vector2 target, float time)
    // {
    //     Vector2 startPos = imageToMove.anchoredPosition;
    //     float elapsedTime = 0;

    //     while (elapsedTime < time)
    //     {
    //         // Calculate how far along we are (0.0 to 1.0)
    //         float t = elapsedTime / time;

    //         // Optional: Make it "Smooth" (Ease In and Out) instead of robotic linear speed
    //         // If you want constant speed, remove this line.
    //         t = Mathf.SmoothStep(0.0f, 1.0f, t);

    //         // Move the image
    //         imageToMove.anchoredPosition = Vector2.Lerp(startPos, target, t);

    //         elapsedTime += Time.deltaTime;
    //         yield return null; // Wait for the next frame
    //     }

    //     // Ensure it lands exactly on the target at the end
    //     imageToMove.anchoredPosition = target;
    // }
}