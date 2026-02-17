using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    public AudioClip explosionSound;

    [Header("Miss Settings")]
    public float missYPosition = -8f;

    private bool isExploded = false;

    // Shared AudioSource (same one used by Fruit)
    private static AudioSource sliceSource;

    void Awake()
    {
        // Cache AudioSource ONCE
        if (sliceSource == null)
        {
            GameObject audioObj = GameObject.Find("SliceAudio");
            if (audioObj != null)
            {
                sliceSource = audioObj.GetComponent<AudioSource>();
            }
          
        }
    }

    public void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        // 🔊 INSTANT SOUND (NO DELAY)
        if (explosionSound != null && sliceSource != null)
        {
           sliceSource.PlayOneShot(explosionSound, 3f);
        }

        // 💥 VISUAL
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (transform.position.y < missYPosition)
        {
            Destroy(gameObject);
        }
    }
}
