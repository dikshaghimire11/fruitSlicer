using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour
{
    [Header("Cutting Settings")]
    public GameObject leftHalf;
    public GameObject rightHalf;
    public float sliceForce = 5f;
    public float rotationForce = 20f;

    [Header("Visuals & Audio")]
    public AudioClip breakSound;

    [Header("Freeze Settings")]
    public float freezeDuration = 1f;

    private bool isSliced = false;

    // Shared AudioSource (same as Fruit & Bomb)
    private static AudioSource sliceSource;

    void Awake()
    {
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

        // 🔊 INSTANT SOUND (NO DELAY)
        if (breakSound != null && sliceSource != null)
        {
            sliceSource.pitch = Random.Range(0.95f, 1.05f);
            sliceSource.PlayOneShot(breakSound);
        }

        StartCoroutine(decreaseSpwanDelay());

        // Hide original ice
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();
        if (sr) sr.enabled = false;
        if (col) col.enabled = false;

        // Spawn pieces
        SpawnSlicedParts(sliceDirection);

        // Freeze effect
        StartCoroutine(FreezeAndShowSnow());
    }

    void SpawnSlicedParts(Vector2 sliceDirection)
    {
        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        if (leftRb && rightRb)
        {
            leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
            rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

            float torque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
            leftRb.AddTorque(torque, ForceMode2D.Impulse);
            rightRb.AddTorque(-torque, ForceMode2D.Impulse);
        }

        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }

    IEnumerator FreezeAndShowSnow()
    {
        startSlowMotion();
        yield return new WaitForSecondsRealtime(freezeDuration);
        stopSlowMotion();
        Destroy(gameObject);
    }

    public IEnumerator decreaseSpwanDelay()
    {
        float originalDelay = FruitSpawner.instance.spawnDelay;
        FruitSpawner.instance.spawnDelay = 0.1f;
        yield return new WaitForSeconds(1f);
        FruitSpawner.instance.spawnDelay = originalDelay;
    }

    void startSlowMotion()
    {
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void stopSlowMotion()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
