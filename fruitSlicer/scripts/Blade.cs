using UnityEngine;

[RequireComponent(typeof(TrailRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
public class Blade : MonoBehaviour
{
    private TrailRenderer bladeTrail;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Camera mainCamera;

    [Header("Blade Settings")]
    public float minSlicingVelocity = 3.5f;
    public float minSliceDistance = 0.15f;
    public float minMovementForSlice = 0.08f;
    public GameObject floatingTextPrefab;

    private bool isSlicing;
    private Vector3 previousPosition;
    private Vector2 currentSliceDirection;
    private float accumulatedDistance;

    [Header("Combo Settings")]
    public float maxComboDelay = 0.2f;
    private int comboCount;
    private float lastHitTime;

    [Header("Blade Audio Settings")]
    public float minSoundVelocity = 2.5f;
    public float maxSoundVelocity = 15f;
    public float audioFadeSpeed = 6f;

    void Awake()
    {
        mainCamera = Camera.main;
        bladeTrail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        bladeTrail.emitting = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Stop();
    }

    void Update()
    {
        if (IsInputDown()) StartSlicing();
        else if (IsInputUp()) StopSlicing();

        if (isSlicing)
            ContinueSlicing();
    }

    bool IsInputDown()
    {
        return Input.GetMouseButtonDown(0) ||
              (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    bool IsInputUp()
    {
        return Input.GetMouseButtonUp(0) ||
              (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

    Vector3 GetInputPosition()
    {
        Vector3 screenPos = Input.touchCount > 0
            ? (Vector3)Input.GetTouch(0).position
            : Input.mousePosition;

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

        accumulatedDistance = 0f;
        comboCount = 0;

        // 🔇 Do NOT auto-play sound on touch
        audioSource.volume = 0f;
    }

    void StopSlicing()
    {
        isSlicing = false;
        bladeTrail.emitting = false;

        audioSource.Stop();
        audioSource.volume = 0f;
    }

    void ContinueSlicing()
    {
        Vector3 worldPos = GetInputPosition();
        Vector3 movement = worldPos - previousPosition;
        float distance = movement.magnitude;

        if (distance <= 0f)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, audioFadeSpeed * Time.deltaTime);
            if (audioSource.volume <= 0.01f)
                audioSource.Stop();
            return;
        }

        float velocity = distance / Time.deltaTime;
        accumulatedDistance += distance;

        currentSliceDirection = movement.normalized;

        // 🎵 Blade sound handling (FIXED)
        if (velocity > minSoundVelocity)
        {
            float targetVolume = Mathf.Clamp01(
                (velocity - minSoundVelocity) / (maxSoundVelocity - minSoundVelocity));

            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, audioFadeSpeed * Time.deltaTime);
            audioSource.pitch = Mathf.Lerp(0.85f, 1.4f, targetVolume);

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, audioFadeSpeed * Time.deltaTime);
            if (audioSource.volume <= 0.01f)
                audioSource.Stop();
        }

        // Check slicing hits
        if (velocity > minSlicingVelocity && distance > minMovementForSlice && accumulatedDistance > minSliceDistance)
        {
            RaycastHit2D[] hits = Physics2D.LinecastAll(previousPosition, worldPos);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.enabled)
                    CheckHit(hit.collider);
            }
        }

        rb.MovePosition(worldPos);
        previousPosition = worldPos;
    }

    void CheckHit(Collider2D other)
    {
        if (!other.enabled) return;

        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            other.enabled = false;
            fruit.Slice(currentSliceDirection);
            bool isCorrectFruit = false;
            switch (ModeManager.Instance.currentMode)
            {
                case GameMode.Infinite:
                    ScoreManager.instance?.AddScore(fruit.points);
                    ShowFloatingText("+" + fruit.points, Color.cyan, fruit.transform.position, 0.5f, 0.2f);
                    break;

                case GameMode.JuiceMaking:
                    isCorrectFruit = JuiceManager.instance?.CheckFruit(fruit.name) ?? false;
                    if (isCorrectFruit)
                    {
                        ShowFloatingText("PERFECT!", Color.cyan, fruit.transform.position, 0.6f, 0.2f);
                    }
                    else
                    {
                        ShowFloatingText("X", Color.red, fruit.transform.position, 1.5f, 0.2f);
                    }
                    break;
            }

            HandleCombo(fruit, isCorrectFruit);
            return;
        }

        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            other.enabled = false;
            bomb.Explode();
            ShowFloatingText("BOOM!", Color.red, bomb.transform.position, 0.6f, 0.25f);
            ScoreManager.instance?.HitBomb();
            comboCount = 0;
            return;
        }

        Ice ice = other.GetComponent<Ice>();
        if (ice != null)
        {
            other.enabled = false;
            ice.Slice(currentSliceDirection);
            ShowFloatingText("FREEZE!", Color.cyan, ice.transform.position, 0.5f, 0.2f);
        }
    }

    void HandleCombo(Fruit fruit, bool isCorrectFruit)
    {
        if (Time.time - lastHitTime > maxComboDelay)
            comboCount = 0;

        lastHitTime = Time.time;

        if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
        {
            if (!isCorrectFruit)
            {
                comboCount = 0;
                return;
            }

            comboCount++;
        }
        else
        {
            comboCount++;
        }

        if (comboCount < 2) return;

        float textSize = 0.6f;
        int bonus;

        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.JuiceMaking:
                bonus = comboCount * 2;
                ScoreManager.instance?.addBonusAmount(bonus);
                ShowFloatingText("+" + bonus, Color.yellow, fruit.transform.position, textSize, 0f);
                break;

            case GameMode.Infinite:
                bonus = comboCount * 5;
                ScoreManager.instance?.AddScore(bonus);
                ShowFloatingText("COMBO", Color.yellow, fruit.transform.position + new Vector3(-0.2f, 0f, 0f), textSize, 0f);
                ShowFloatingText("+" + bonus, Color.yellow, fruit.transform.position + new Vector3(0.2f, 0f, 0f), textSize, 0f);
                break;
        }

        if (SoundManager.instance != null)
        {
            int cappedCombo = Mathf.Min(comboCount, 3);

            float pitch = 0.75f + (cappedCombo - 2) * 0.08f;
            pitch = Mathf.Clamp(pitch, 0.75f, 0.95f);

            SoundManager.instance.PlayComboSound(pitch);
        }
    }


    public void ShowFloatingText(string message, Color color, Vector3 position, float size, float yOffset)
    {
        if (floatingTextPrefab == null) return;
        GameObject obj = Instantiate(
            floatingTextPrefab,
            position + new Vector3(0f, yOffset, 0f),
            Quaternion.identity
        );

        obj.transform.localScale = Vector3.one * size;

        FloatingText ft = obj.GetComponent<FloatingText>();
        if (ft != null)
            ft.Setup(message, color);
    }
}
