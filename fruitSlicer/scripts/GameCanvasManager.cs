using UnityEngine;
using UnityEngine.UI; // Required for UI
using System.Collections;

public class GameCanvasManager : MonoBehaviour
{
    public RectTransform imageToMove; // Drag your Image here
    public Vector2 targetPosition;    // Set X and Y in Inspector (e.g., 0, 0 for center)
    public float duration = 2.0f;     // Duration in seconds

    // Call this function to start the movement (e.g., via a Button)
    void Start()
    {
        StartMoving();
    }
    public void StartMoving()
    {
        StartCoroutine(MoveRoutine(targetPosition, duration));
    }

    private IEnumerator MoveRoutine(Vector2 target, float time)
    {
        Vector2 startPos = imageToMove.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Calculate how far along we are (0.0 to 1.0)
            float t = elapsedTime / time;

            // Optional: Make it "Smooth" (Ease In and Out) instead of robotic linear speed
            // If you want constant speed, remove this line.
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            // Move the image
            imageToMove.anchoredPosition = Vector2.Lerp(startPos, target, t);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it lands exactly on the target at the end
        imageToMove.anchoredPosition = target;
    }
}