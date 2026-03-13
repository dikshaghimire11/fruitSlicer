using UnityEngine;
using System.Collections;

public class BladeManager : MonoBehaviour
{
    public static BladeManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SwapBlade(GameObject currentBlade, GameObject powerBladePrefab, GameObject normalBladePrefab, float duration)
    {
        StartCoroutine(SwapCoroutine(currentBlade, powerBladePrefab, normalBladePrefab, duration));
    }

    private IEnumerator SwapCoroutine(GameObject currentBlade, GameObject powerBladePrefab, GameObject normalBladePrefab, float duration)
    {
        Transform parent = currentBlade.transform.parent;
        Vector3 position = currentBlade.transform.position;

        // Spawn power blade
        GameObject powerBlade = Instantiate(powerBladePrefab, position, Quaternion.identity, parent);

        // Disable current blade
        currentBlade.SetActive(false);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Spawn normal blade
        Instantiate(normalBladePrefab, powerBlade.transform.position, Quaternion.identity, parent);

        // Destroy power blade and old blade
        Destroy(powerBlade);
        Destroy(currentBlade);
    }
}