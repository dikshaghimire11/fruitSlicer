using UnityEngine;
using TMPro; // Important: This line allows us to use TextMeshPro

public class FrameRateCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Drag your text object here in Inspector
    private float pollingTime = 1f; // Update every 1 second
    private float time;
    private int frameCount;

    void Update()
    {
        // Count frames
        time += Time.unscaledDeltaTime;
        frameCount++;

        // If 1 second has passed, update the text
        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = frameRate.ToString() + " FPS";

            // Reset timers
            time -= pollingTime;
            frameCount = 0;
        }
    }
}