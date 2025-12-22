using UnityEngine;

[RequireComponent(typeof(TrailRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] 
public class Blade : MonoBehaviour
{
    [Header("Components")]
    private TrailRenderer bladeTrail;
    private CircleCollider2D bladeCollider;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Slicing Settings")]
    public float minSlicingVelocity = 0.01f;
    public AudioClip sliceSound; 

    private bool isSlicing = false;
    private Vector3 previousPosition;
    private Vector2 currentSliceDirection;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        bladeTrail = GetComponent<TrailRenderer>();
        bladeCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        bladeCollider.isTrigger = true;
        bladeTrail.emitting = false;
        bladeCollider.enabled = false;
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
        bladeCollider.enabled = true;
        previousPosition = GetInputPosition();
        rb.position = previousPosition;
    }

  void ContinueSlicing()
{
    Vector3 worldPos = GetInputPosition();
    Vector3 movement = worldPos - previousPosition;
    
    if (movement.magnitude > 0)
    {
        currentSliceDirection = movement.normalized;
    }

    // CHANGE THIS LINE: Use unscaledDeltaTime so the blade works while frozen!
    float velocity = movement.magnitude / Time.unscaledDeltaTime; 
    
    bladeCollider.enabled = velocity > minSlicingVelocity;

    rb.MovePosition(worldPos);
    previousPosition = worldPos;
}

    void StopSlicing()
    {
        isSlicing = false;
        bladeTrail.emitting = false;
        bladeCollider.enabled = false;
    }

  private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check for FRUIT
        Fruit fruit = collision.gameObject.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.Slice(currentSliceDirection);


            ScoreManager.instance.AddScore(5);
            // Optional: Play slice sound if assigned here
            // if (audioSource != null && sliceSound != null) audioSource.PlayOneShot(sliceSound);
            return; // Stop checking, we hit a fruit
        }

        // 2. Check for BOMB
        Bomb bomb = collision.gameObject.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Explode();
            return;
        }

        // 3. Check for ICE (Updated)
        Ice ice = collision.gameObject.GetComponent<Ice>();
        if (ice != null)
        {
            // NEW: We pass 'currentSliceDirection' so it cuts properly!
            ice.Slice(currentSliceDirection);
        }
    }
}