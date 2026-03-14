using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class BladeManager : MonoBehaviour
{
    public static BladeManager instance;

    public TextMeshProUGUI bladeTimer;

    private Coroutine coroutine;

    private float bladeLifeTime = 0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SwapBlade(GameObject currentBlade, GameObject powerBladePrefab, GameObject normalBladePrefab, float duration)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SwapCoroutine(currentBlade, powerBladePrefab, normalBladePrefab, duration));


    }

    private IEnumerator SwapCoroutine(GameObject currentBlade, GameObject powerBladePrefab, GameObject normalBladePrefab, float duration)
    {
        bladeLifeTime = duration;
        GameObject bladeLifeTimeParent = bladeTimer.transform.parent.gameObject;
        bladeLifeTimeParent.SetActive(true);

        // Transform parent = currentBlade.transform.parent;
        Vector3 position = currentBlade.transform.position;
        IBladeSpecialAbility specialAbility = currentBlade.GetComponent<IBladeSpecialAbility>();
        specialAbility.stopAttacking();

        // Spawn power blade
        GameObject powerBlade = Instantiate(powerBladePrefab, position, Quaternion.identity, GameCanvasManager.instance.transform);
        Blade blade = powerBlade.GetComponent<Blade>();
        blade.StartSlicing();
        IBladeSpecialAbility specialAbility1 = powerBlade.GetComponent<IBladeSpecialAbility>();
        // currentBlade.SetActive(false);
        GameObject.Destroy(currentBlade);
        yield return null;
        specialAbility1.powerReady();
        // Disable current blade


        // Wait for duration

        yield return new WaitForSeconds(duration);
        bladeLifeTimeParent.SetActive(false);

        // Spawn normal blade

        specialAbility1.stopAttacking();
        GameObject normalBlade = Instantiate(normalBladePrefab, powerBlade.transform.position, Quaternion.identity, GameCanvasManager.instance.transform);
        powerBlade.SetActive(false);
        Blade blade1 = normalBlade.GetComponent<Blade>();
        blade1.StartSlicing();
        // Destroy power blade and old blade
        // Destroy(currentBlade);
        Destroy(powerBlade);
    }

    void Update()
    {
        if (bladeLifeTime >= 0)
        {
            bladeLifeTime -= Time.deltaTime;
            bladeTimer.text = ((int)bladeLifeTime).ToString();
        }


    }
}