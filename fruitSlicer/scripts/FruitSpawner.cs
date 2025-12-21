using UnityEngine; // Required for Unity-specific functionalities (GameObject, Instantiate, etc.)
using System.Collections; // Required for Coroutines (IEnumerator, WaitForSeconds)
using System.Collections.Generic;
public class FruitSpawner : MonoBehaviour
{
    // Public variables accessible in the Inspector
    public List<GameObject> fruitPrefabs;
    public float spawnDelay = 2f;  // Time between fruit spawns
    public float spawnForce = 10f; // How much force to apply upwards
    public float minX = -2f;       // Minimum X position for spawning
    public float maxX = 2f;
    public GameObject gameContainer;     // Maximum X position for spawning

    // Called when the script instance is being loaded (once)
    void Start()
    {
        // Start the coroutine to repeatedly spawn fruits
        StartCoroutine(SpawnFruitsRoutine());



    }

    // Coroutine to handle the spawning logic with delays
    IEnumerator SpawnFruitsRoutine()
    {
        Vector3[] corners = new Vector3[4];


        gameContainer.GetComponent<RectTransform>().GetWorldCorners(corners);
        // This loop will run indefinitely as long as the GameObject is active
        while (true)
        {

            // Wait for the specified spawn delay before spawning the next fruit
            yield return new WaitForSeconds(spawnDelay);
            if (fruitPrefabs.Count > 0)
            {
                int randomIndex = Random.Range(0, fruitPrefabs.Count);
                GameObject randomFruitPrefab = fruitPrefabs[randomIndex];

                // 1. Calculate a random X position within our defined range
                float randomX = Random.Range(corners[0].x, corners[3].x);

                // 2. Determine the spawn position (just above the screen)
                // We'll spawn at a fixed Y position, and Z=0 for 2D.
                // You might need to adjust the Y position based on your camera's Orthographic Size.
                // For a camera with Orthographic Size 5, Y=6 might be a good starting point.
                Vector3 spawnPosition = new Vector3(randomX, -3f, -10f); // Spawns from the bottom for an upward slice!
                                                                         // Let's adjust this for a proper "up-into-view" spawn.
                                                                         // Let's change this to be *above* the screen for now, for fruits to fall.
                                                                         // A better approach for Fruit Slicer is to spawn from the bottom and apply upward force.
                                                                         // Let's change the Y position to be *below* the screen, and apply an *upward* force.
                                                                         // Let's set Y to -6 (below despawn zone) and apply upward force.

                // Let's reconsider the spawn logic for a typical fruit slicer: fruits "pop up" from below.
                // So, spawnPosition should be below the screen. The fruit will then get an upward force.

                // For a typical Fruit Slicer: fruits appear from below and fly upwards.
                // Let's assume your bottom despawn zone is around Y=-6 to -7.
                // So, spawn just below that, e.g., Y = -7.
                spawnPosition = new Vector3(randomX, -3f, -10f);


                // 3. Instantiate the fruit prefab at the calculated position
                GameObject newFruit = Instantiate(randomFruitPrefab, spawnPosition, Quaternion.identity);

                // Quaternion.identity means "no rotation" or "default rotation"

                // 4. Get the Rigidbody2D component of the new fruit
                Rigidbody2D rb = newFruit.GetComponent<Rigidbody2D>();

                // 5. Apply an upward force to the fruit if it has a Rigidbody2D
                if (rb != null)
                {
                    // Apply a force vector: (0, spawnForce) means pure upward force
                    // ForceMode2D.Impulse applies an instant force
                    rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);

                    // Add some random horizontal force as well, to make it more dynamic
                    float randomHorizontalForce = Random.Range(-spawnForce / 2, spawnForce / 2); // Small random sideways push
                    rb.AddForce(new Vector2(randomHorizontalForce, 0), ForceMode2D.Impulse);
                }
                Destroy(newFruit, 5f);
            }
        }
    }
}
