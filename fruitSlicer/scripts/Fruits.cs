using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject leftHalf;
    public GameObject rightHalf;

    public float sliceForce = 5f;
    public float rotationForce = 20f;

    public AudioClip sliceSound;
    public int points = 10;
    public float missYPosition = -8f;

    public Color juiceColor;

    private bool isSliced = false;

    // Shared AudioSource for all fruits
    private static AudioSource sliceSource;

    void Awake()
    {
        // Cache AudioSource ONCE (no runtime searching during slice)
        if (sliceSource == null)
        {
            GameObject audioObj = GameObject.Find("SliceAudio");
            if (audioObj != null)
            {
                sliceSource = audioObj.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("SliceAudio GameObject not found in scene!");
            }
        }
    }

    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;

        // PLAY SOUND INSTANTLY (NO DELAY)
        if (sliceSound != null && sliceSource != null)
        {
            sliceSource.pitch = Random.Range(0.95f, 1.05f); // optional juicy effect
            sliceSource.PlayOneShot(sliceSound);
        }

        // Hide main fruit
        gameObject.SetActive(false);

        // Spawn halves
        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
        rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

        float torque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
        leftRb.AddTorque(torque, ForceMode2D.Impulse);
        rightRb.AddTorque(-torque, ForceMode2D.Impulse);

        // Cleanup
        Destroy(gameObject);
        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }

    void Update()
    {
        if (transform.position.y < missYPosition)
        {
            if (ModeManager.Instance.currentMode == GameMode.Infinite)
            {
                if (ScoreManager.instance != null)
                    ScoreManager.instance.LoseLife();
            }
            else
            {
                bool isTargetFruit =
           JuiceManager.instance != null &&
           JuiceManager.instance.targetFruitNew != null &&
           gameObject.name.StartsWith(JuiceManager.instance.targetFruitNew.name);
                if (isTargetFruit)
                {
                    if(SoundManager.instance != null)
                    {
                        SoundManager.instance.PlayMissTargetedFruitSound();
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
