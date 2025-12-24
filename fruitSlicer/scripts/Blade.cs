using UnityEngine;

[RequireComponent(typeof(TrailRenderer), typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Blade : MonoBehaviour
{
    [Header("Components")]
    private TrailRenderer bladeTrail;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Slicing Settings")]
    public float minSlicingVelocity = 5.0f;
    public float minSliceDistance = 0.1f;
    
    [Header("Combo Settings")]
    public float maxComboDelay = 0.2f; 
    
    [Header("Visual Effects")]
    public GameObject floatingTextPrefab; 

    private bool isSlicing = false;
    private Vector3 previousPosition;
    private Vector2 currentSliceDirection;
    private Camera mainCamera;
    
    // --- COMBO VARIABLES ---
    private int comboCount = 0; 
    private float lastHitTime = 0f; 

    void Awake()
    {
        mainCamera = Camera.main;
        bladeTrail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        bladeTrail.emitting = false;
        
        Collider2D col = GetComponent<Collider2D>();
        if(col != null) col.enabled = false; 
    }

    void Update()
    {
        if (IsInputDown()) StartSlicing();
        else if (IsInputUp()) StopSlicing();

        if (isSlicing) ContinueSlicing();
    }

    bool IsInputDown()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    bool IsInputUp()
    {
        return Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

    Vector3 GetInputPosition()
    {
        Vector3 screenPos;
        if (Input.touchCount > 0) screenPos = Input.GetTouch(0).position;
        else screenPos = Input.mousePosition;

        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    void StartSlicing()
    {
        isSlicing = true;
        bladeTrail.Clear();
        bladeTrail.emitting = true;
        previousPosition = GetInputPosition();
        rb.position = previousPosition;
        
        // Reset combo on new swipe
        comboCount = 0; 
        lastHitTime = 0f;
    }

    void ContinueSlicing()
    {
        Vector3 worldPos = GetInputPosition();
        Vector3 directionV3 = worldPos - previousPosition;
        float distance = directionV3.magnitude;
        float velocity = distance / Time.unscaledDeltaTime;

        if (velocity > minSlicingVelocity && distance > minSliceDistance)
        {
             currentSliceDirection = directionV3.normalized;

             RaycastHit2D[] hits = Physics2D.LinecastAll(previousPosition, worldPos);

             foreach (RaycastHit2D hit in hits)
             {
                 if (hit.collider != null) CheckHit(hit.collider);
             }
        }

        rb.MovePosition(worldPos);
        previousPosition = worldPos;
    }

    void StopSlicing()
    {
        isSlicing = false;
        bladeTrail.emitting = false;
    }

    void CheckHit(Collider2D other)
    {
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.Slice(currentSliceDirection);
            
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(fruit.points);
            ShowFloatingText("+" + fruit.points, new Color(0.278f, 0.572f, 0.866f), fruit.transform.position);

            if (Time.time - lastHitTime > maxComboDelay)
            {
                comboCount = 0; 
            }

            // Update timer and count
            lastHitTime = Time.time;
            comboCount++;

            // Only trigger combo if we have hit 2 or more IN THIS SHORT WINDOW
            if (comboCount >= 2)
            {
                int bonusPoints = comboCount * 5; 

                if (ScoreManager.instance != null)
                {
                    ScoreManager.instance.AddScore(bonusPoints);
                }

                ShowFloatingText("COMBO " + bonusPoints, new Color(1.0f, 0.831f, 0.039f), fruit.transform.position);
            }

            return;
        }

        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Explode();
            ShowFloatingText("BOOM!", new Color(0.925f, 0.247f, 0.235f), bomb.transform.position);
            comboCount = 0; 
            return;
        }

        Ice ice = other.GetComponent<Ice>();
        if (ice != null)
        {
            ice.Slice(currentSliceDirection);
            ShowFloatingText("FREEZE!", Color.cyan, ice.transform.position);
        }
    }

    void ShowFloatingText(string message, Color color, Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = new Vector3(position.x, position.y + 1f, -5f);
            
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            
            FloatingText floatingTextScript = textObj.GetComponent<FloatingText>();
            if (floatingTextScript != null)
            {
                floatingTextScript.Setup(message, color);
            }
        }
    }
}