using UnityEngine;
using System.Collections; // Required for IEnumerator and Coroutines

public class SlicedFruitPart : MonoBehaviour
{
    [Header("Lifetime Settings")]
    public float fadeDelay = 2f; // Time before fading starts
    public float fadeDuration = 1f; // How long it takes to fade out
    public float destroyDelay = 3f; // Total time before the object is destroyed

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing SpriteRenderer!");
            Destroy(this);
            return;
        }

        StartCoroutine(FadeAndDestroyRoutine());
    }


    IEnumerator FadeAndDestroyRoutine()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(fadeDelay);

        // --- Start Fading Out ---
        float timer = 0f;
        Color startColor = spriteRenderer.color; // Get the current color
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Target transparent color

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Interpolate the color's alpha value over time
            spriteRenderer.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null; // Wait for the next frame
        }
        spriteRenderer.color = endColor; // Ensure it's fully transparent

        // --- Destroy the GameObject ---
        // Wait for any remaining time until the total destroyDelay is reached
        // (destroyDelay - fadeDelay - fadeDuration)
        float remainingDestroyTime = destroyDelay - (fadeDelay + fadeDuration);
        if (remainingDestroyTime > 0)
        {
            yield return new WaitForSeconds(remainingDestroyTime);
        }

        Destroy(gameObject); // Destroy the fruit part
    }
}
