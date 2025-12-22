using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour
{
    [Header("Cutting Settings")]
    public GameObject leftHalf;  // Drag the LEFT half of the ice here
    public GameObject rightHalf; // Drag the RIGHT half of the ice here
    public float sliceForce = 5f;
    public float rotationForce = 20f;
    
    [Header("Visuals & Audio")]
    public AudioClip breakSound;
    
    [Header("Freeze Settings")]
    public float freezeDuration = 1f;

    private bool isSliced = false;

    // We now accept the 'direction' of the cut, just like Fruit.cs
    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;

        // 1. Play Sound
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position, 1.0f);
        }

        // 2. Hide the main Ice Cube immediately
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // 3. Create the Cut Pieces (Physics Logic)
        SpawnSlicedParts(sliceDirection);

        // 4. Start the Freeze & Snow Effect
        StartCoroutine(FreezeAndShowSnow());
    }

    void SpawnSlicedParts(Vector2 sliceDirection)
    {
        // Instantiate the two halves at the current position
        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        if (leftRb != null && rightRb != null)
        {
            // Push them apart (Same math as Fruit.cs)
            leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
            rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

            // Add rotation so they spin
            float currentTorque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
            leftRb.AddTorque(currentTorque, ForceMode2D.Impulse);
            rightRb.AddTorque(-currentTorque, ForceMode2D.Impulse);
        }

        // Clean up the pieces after 4 seconds
        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }

    IEnumerator FreezeAndShowSnow()
    {
        // A. Show Snow Overlay (if it exists)
        GameObject snowScreen = GameObject.Find("SnowOverlay");
        if (snowScreen != null) snowScreen.SetActive(true);

        // B. Freeze Time
        Time.timeScale = 0.5f;

        // C. Wait (using Realtime)
        yield return new WaitForSecondsRealtime(freezeDuration);

        // D. Unfreeze
        Time.timeScale = 1f;

        // E. Hide Snow Overlay
        if (snowScreen != null) snowScreen.SetActive(false);

        // F. Destroy the original object
        Destroy(gameObject);
    }
}