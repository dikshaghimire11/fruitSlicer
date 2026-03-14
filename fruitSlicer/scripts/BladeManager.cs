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
        IBladeSpecialAbility specialAbility = currentBlade.GetComponent<IBladeSpecialAbility>();
        specialAbility.stopAttacking();

        // Spawn power blade
        GameObject powerBlade = Instantiate(powerBladePrefab, position, Quaternion.identity, parent);
        Blade blade = powerBlade.GetComponent<Blade>();
        blade.StartSlicing();
        // Disable current blade
        currentBlade.SetActive(false);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Spawn normal blade
        IBladeSpecialAbility specialAbility1 = powerBlade.GetComponent<IBladeSpecialAbility>();
        specialAbility1.stopAttacking();
        GameObject normalBlade = Instantiate(normalBladePrefab, powerBlade.transform.position, Quaternion.identity, parent);
        powerBlade.SetActive(false);
        Blade blade1 = normalBlade.GetComponent<Blade>();
        blade1.StartSlicing();
        // Destroy power blade and old blade
        Destroy(powerBlade);
        Destroy(currentBlade);
    }
}