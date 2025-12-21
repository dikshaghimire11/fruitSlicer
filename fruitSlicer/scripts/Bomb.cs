using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Drag your explosion particle here
    public AudioClip explosionSound;   // Drag your boom sound here
    
    private bool isExploded = false;

    // This function is called by the Blade when it hits the bomb
    public void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        // 1. Play Sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        }

        // 2. Spawn Explosion Effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}