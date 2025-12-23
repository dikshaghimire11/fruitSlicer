using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Drag your explosion particle here
    public AudioClip explosionSound;   // Drag your boom sound here
    
    private bool isExploded = false;
    [Header("Miss Settings")]
    public float missYPosition = -8f;


    public void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        // 1. Play Effects (Keep your existing code)
        if (explosionSound != null) AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 2. Report to Manager (NEW)
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.HitBomb();
        }

        Destroy(gameObject);
    }
     void Update()
{
    // If the fruit falls below the limit...
    if (transform.position.y < missYPosition)
    {   
        // 2. NOW destroy it
        Destroy(gameObject);
    }
}
}