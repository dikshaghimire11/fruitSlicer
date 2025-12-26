using UnityEngine;

[RequireComponent(typeof(TrailRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
public class Blade : MonoBehaviour
{
    private TrailRenderer bladeTrail;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Camera mainCamera;

    [Header("Blade Settings")]
    public float minSlicingVelocity = 5.0f;
    public float minSliceDistance = 0.1f;
    public GameObject floatingTextPrefab; 
    
    private bool isSlicing = false;
    private Vector3 previousPosition;
    private Vector2 currentSliceDirection;
    
    private int comboCount = 0; 
    private float lastHitTime = 0f; 
    public float maxComboDelay = 0.2f;

    void Awake()
    {
        mainCamera = Camera.main;
        bladeTrail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        bladeTrail.emitting = false;
        
        // Ensure the blade itself doesn't physically push objects
        if(GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false; 
    }

    void Update()
    {
        if (IsInputDown()) StartSlicing();
        else if (IsInputUp()) StopSlicing();
        if (isSlicing) ContinueSlicing();
    }

    bool IsInputDown() { return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began); }
    bool IsInputUp() { return Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended); }
    
    Vector3 GetInputPosition() 
    {
        Vector3 screenPos = (Input.touchCount > 0) ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    void StartSlicing() { isSlicing = true; bladeTrail.Clear(); bladeTrail.emitting = true; previousPosition = GetInputPosition(); rb.position = previousPosition; comboCount = 0; }
    void StopSlicing() { isSlicing = false; bladeTrail.emitting = false; }

    void ContinueSlicing()
    {
        Vector3 worldPos = GetInputPosition();
        Vector3 directionV3 = worldPos - previousPosition;
        float distance = directionV3.magnitude;
        float velocity = distance / Time.unscaledDeltaTime;

        if (velocity > minSlicingVelocity && distance > minSliceDistance)
        {
             currentSliceDirection = directionV3.normalized;
             // Check for hits between the last frame position and current position
             RaycastHit2D[] hits = Physics2D.LinecastAll(previousPosition, worldPos);
             foreach (RaycastHit2D hit in hits) if (hit.collider != null) CheckHit(hit.collider);
        }
        rb.MovePosition(worldPos);
        previousPosition = worldPos;
    }

    void CheckHit(Collider2D other)
    {
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            // 1. Visual Slice (If this happens, physics works)
            fruit.Slice(currentSliceDirection);

            // --- DEBUG START ---
            Debug.Log("Hit Fruit! Current Mode: " + ModeManager.Instance.currentMode);
            // -------------------

            if (ModeManager.Instance.currentMode == GameMode.Infinite)
            {
                Debug.Log("Mode is Infinite. Adding Score...");
                if (ScoreManager.instance != null) ScoreManager.instance.AddScore(fruit.points);
                ShowFloatingText("+" + fruit.points, new Color(0.278f, 0.572f, 0.866f), fruit.transform.position);
                HandleCombo(fruit);
            }
            else if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                Debug.Log("Mode is JuiceMaking. Calling JuiceManager...");
                
                if (JuiceManager.instance != null) 
                {
                    Debug.Log("JuiceManager Found! Sending Data...");
                    JuiceManager.instance.CheckFruit(fruit.fruitType);
                }
                else
                {
                    Debug.LogError("CRITICAL ERROR: JuiceManager is NULL! It is not in the scene.");
                }

                ShowFloatingText("SPLASH!", Color.cyan, fruit.transform.position);
            }
            return;
        }

        // 2. Check if we hit a BOMB
        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Explode(); // Play sound/visuals
            ShowFloatingText("BOOM!", Color.red, bomb.transform.position);
            
            // --- BOMB LOGIC: ALWAYS LOSE LIFE ---
            if (ScoreManager.instance != null) 
            {
                Debug.Log("Bomb Hit! Reducing Life.");
                ScoreManager.instance.HitBomb(); // Calls LoseLife()
            }
            else
            {
                Debug.LogError("ScoreManager is missing! Cannot reduce life.");
            }
            // ------------------------------------
            
            comboCount = 0; 
            return;
        }

        // 3. Check if we hit ICE
        Ice ice = other.GetComponent<Ice>();
        if (ice != null)
        {
            ice.Slice(currentSliceDirection);
            ShowFloatingText("FREEZE!", Color.cyan, ice.transform.position);
        }
    }

    void HandleCombo(Fruit fruit)
    {
        if (Time.time - lastHitTime > maxComboDelay) comboCount = 0; 
        lastHitTime = Time.time;
        comboCount++;
        if (comboCount >= 2)
        {
            int bonus = comboCount * 5; 
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(bonus);
            ShowFloatingText("COMBO " + bonus, Color.yellow, fruit.transform.position);
        }
    }

    public void ShowFloatingText(string message, Color color, Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, position + Vector3.up, Quaternion.identity);
            FloatingText ft = textObj.GetComponent<FloatingText>();
            if (ft != null) ft.Setup(message, color);
        }
    }
}