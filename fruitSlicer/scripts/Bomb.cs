using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; 
    public AudioClip explosionSound;   
    
    private bool isExploded = false;
    [Header("Miss Settings")]
    public float missYPosition = -8f;

    public void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        // Visuals Only
        if (explosionSound != null) AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // NOTE: Score/Life logic is now inside Blade.cs CheckHit()
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