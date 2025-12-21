using UnityEngine;
using System.Collections;

public class SliceFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float delayBeforeFade = 3f;
    public float fadeDuration = 1f;

    [Header("Juice Settings")]
    public GameObject juicePrefab; 
    
    // Manual color override
    public Color specificJuiceColor = Color.white; 
    public bool useSpriteColor = false; 

    // NEW: Controls for "More" Juice
    [Range(0.5f, 2f)] public float scaleMultiplier = 1.0f; // Make it bigger!
    public bool randomizeRotation = true; // Add random tilt

    private void Start()
    {
        if (juicePrefab != null)
        {
            SpawnJuice();
        }
        StartCoroutine(FadeRoutine());
    }

    void SpawnJuice()
    {
        // 1. Determine Rotation
        Quaternion spawnRotation = transform.rotation;
        if (randomizeRotation)
        {
            // Add a random slight tilt (e.g., +/- 15 degrees) to make it look organic
            float randomZ = Random.Range(-15f, 15f);
            spawnRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + randomZ);
        }

        // 2. Instantiate
        GameObject juiceObj = Instantiate(juicePrefab, transform.position, spawnRotation);
        
        // 3. Apply Scaling (Make it bigger/smaller randomly for variety)
        float randomScale = Random.Range(scaleMultiplier * 0.8f, scaleMultiplier * 1.2f);
        juiceObj.transform.localScale = Vector3.one * randomScale;

        // 4. Destroy after time
        Destroy(juiceObj, 2f);

        // 5. Apply Colors to ALL parts of the particle system
        Color finalColor = specificJuiceColor;
        
        // If we want to use the sprite's original tint:
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (useSpriteColor && sr != null)
        {
            finalColor = sr.color;
        }

        // Apply this color to the main system AND all child systems (droplets, mist, etc.)
        ParticleSystem[] allParticles = juiceObj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in allParticles)
        {
            var main = ps.main;
            main.startColor = finalColor;
        }
    }

    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            Color startColor = sr.color;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}       