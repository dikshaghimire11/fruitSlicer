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

    [Header("Blade Audio Settings")]
    public float minSoundVelocity = 5.0f;
    public float maxSoundVelocity = 20f;
    public float audioFadeSpeed = 5f; // Speed of volume fade in/out

    void Awake()
    {
        mainCamera = Camera.main;
        bladeTrail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        bladeTrail.emitting = false;

        // Disable collider if exists
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        audioSource.loop = true; // Loop the swoosh for smooth effect
        audioSource.volume = 0f; // Start muted
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
        Vector3 screenPos = (Input.touchCount > 0) ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
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
        comboCount = 0;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void StopSlicing()
    {
        isSlicing = false;
        bladeTrail.emitting = false;
    }

    void ContinueSlicing()
    {
        Vector3 worldPos = GetInputPosition();
        Vector3 directionV3 = worldPos - previousPosition;
        float distance = directionV3.magnitude;
        float velocity = distance / Time.unscaledDeltaTime;

        if (distance > minSliceDistance)
            currentSliceDirection = directionV3.normalized;
        if (velocity > minSoundVelocity)
        {
            float targetVolume = Mathf.Clamp01((velocity - minSoundVelocity) / (maxSoundVelocity - minSoundVelocity));
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, audioFadeSpeed * Time.unscaledDeltaTime);
            float targetPitch = Mathf.Clamp(velocity / maxSoundVelocity, 0.8f, 1.5f);
            audioSource.pitch = Mathf.MoveTowards(audioSource.pitch, targetPitch, audioFadeSpeed * Time.unscaledDeltaTime);

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, audioFadeSpeed * Time.unscaledDeltaTime);
        }

        if (velocity > minSlicingVelocity && distance > minSliceDistance)
        {
            RaycastHit2D[] hits = Physics2D.LinecastAll(previousPosition, worldPos);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                    CheckHit(hit.collider);
            }
        }

        rb.MovePosition(worldPos);
        previousPosition = worldPos;
    }

    void CheckHit(Collider2D other)
    {
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.Slice(currentSliceDirection);

            if (ModeManager.Instance.currentMode == GameMode.Infinite)
            {
                if (ScoreManager.instance != null)
                    ScoreManager.instance.AddScore(fruit.points);

                ShowFloatingText("+" + fruit.points, new Color(0.278f, 0.572f, 0.866f), fruit.transform.position);
                HandleCombo(fruit);
            }
            else if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                if (JuiceManager.instance != null)
                    JuiceManager.instance.CheckFruit(fruit.gameObject.name);

                ShowFloatingText("SPLASH!", Color.cyan, fruit.transform.position);
            }
            return;
        }

        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Explode();
            ShowFloatingText("BOOM!", Color.red, bomb.transform.position);
            if (ScoreManager.instance != null)
                ScoreManager.instance.HitBomb();

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

    void HandleCombo(Fruit fruit)
    {
        if (Time.time - lastHitTime > maxComboDelay)
            comboCount = 0;

        lastHitTime = Time.time;
        comboCount++;

        if (comboCount >= 2)
        {
            int bonus = comboCount * 5;
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddScore(bonus);

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
