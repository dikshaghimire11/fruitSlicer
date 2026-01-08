using UnityEngine;
using UnityEngine.UI; // Required for UI
using System.Collections;
using System;
using NUnit.Framework;
using Random = UnityEngine.Random;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;

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

    public GameObject workDesk;

    private GameObject newObject;

    public GameObject missionPanel;

    public GameObject freeModeCharacter;

    public GameObject missionAccomplishedPanel;

    public GameObject juiceContainer;

    public Image juiceColor;

    public GameObject addLifeOnlyObject;

    public GameObject addLifeAndTimeObject;





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
        GameObject character;
        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            int length = ShopLists.instance.characterList.Length;
            character = ShopLists.instance.characterList[Random.Range(0, length)];
            choppingBoard.SetActive(false);
        }
        else
        {
            character = freeModeCharacter;
        }

        newObject = Instantiate(character, character.transform.position, character.transform.rotation, shopGameObject.transform);
        newObject.transform.SetSiblingIndex(0);
        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            addLifeAndTimeObject.SetActive(true);
        }
        else
        {
            addLifeOnlyObject.SetActive(true);
        }
        if (SoundManager.instance != null) SoundManager.instance.PlayCharacterPopSound();
        StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 1, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0, -1.99f), 1, 0, workDesk));

    }



    public void missionAccepted()
    {


        missionPanel.SetActive(false);

        startSpawnAction();
        if (SoundManager.instance != null) SoundManager.instance.PlayCharacterGoneSound();
        StartCoroutine(MoveRoutine(new Vector2(-2.67f, -0.87f), 1, 2, newObject));
        StartCoroutine(MoveRoutine(new Vector2(0f, -6f), 1, 2, workDesk));

        // choppingBoard.SetActive(true);

    }

    public void startSpawnAction()
    {
        startSpawning = true;
        FruitSpawner.instance.startSpawnning();
    }
    // public void StartMoving()
    // {
    //     // StartCoroutine(MoveRoutine(targetPosition, duration));
    // }

    private IEnumerator MoveRoutine(Vector2 target, float time, int type, GameObject targetObject)
    // 1 is for enter the screen, 2 is for exit the screen
    {
        RectTransform rectTransform = targetObject.GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Calculate how far along we are (0.0 to 1.0)
            float t = elapsedTime / time;

            // Optional: Make it "Smooth" (Ease In and Out) instead of robotic linear speed
            // If you want constant speed, remove this line.
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            // Move the image
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, target, t);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it lands exactly on the target at the end
        rectTransform.anchoredPosition = target;

        if (type == 1)
        {
            missionPanel.SetActive(true);
            // shopGameObject.SetActive(false);
        }


    }

    public void missionAccomplished()
    {
        if (missionAccomplishedPanel != null)
        {
            Color colorofJuice = JuiceManager.instance.targetFruitNew.GetComponent<Fruit>().juiceColor;
            colorofJuice.a = 100;
            juiceColor.color = colorofJuice;
            if (SoundManager.instance != null) SoundManager.instance.PlayCharacterPopSound();
            StartCoroutine(MoveRoutine(new Vector2(-0.35f, -0.87f), 1, 0, newObject));
            StartCoroutine(MoveRoutine(new Vector2(0, -1.99f), 1, 0, workDesk));
            StartCoroutine(MoveRoutine(new Vector2(0, -1.25f), 1, 0, juiceContainer));

            missionAccomplishedPanel.SetActive(true);
        }
    }

    public void nextOrderAfterMissionAccomplished()
    {
        if (missionAccomplishedPanel != null)
        {

            missionAccomplishedPanel.SetActive(false);
            ScoreManager.instance.RestartGame();
        }
    }

    public void disableAllErrorMessages()
    {
        GameObject[] errorMessages = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in errorMessages)
        {
            if (obj.tag == "ErrorMessage")
            {
                GameObject.Destroy(obj);
            }
        }
    }

    public void getMoreLife()
    {
        AdsManager.instance.currentAdvertisement = AdsManager.AdvertisementType_.AddLife;

        AdsManager.instance.onClickAddLife();
    }

    public void get2xReward()
    {
        AdsManager.instance.currentAdvertisement = AdsManager.AdvertisementType_.x2Reward;
        AdsManager.instance.onClickAddLife();
    }
}