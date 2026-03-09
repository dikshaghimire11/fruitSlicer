using UnityEngine;

public class LaserController : MonoBehaviour

{

    private GameObject target;

    private LineRenderer lineRenderer;

    private IBladeSpecialAbility reportTo;

    private float damageValue;
    private Blade blade;

    public GameObject hitParticlePrefab;

    private GameObject particleEffect;

    private Collider2D collider;

    public AudioClip laserShootSound;

    public AudioSource selfAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            targetDestroyed();
            return;

        }
        Fruit fruit = target.GetComponent<Fruit>();
        if (fruit == null)
            return;
        selfAudioSource.PlayOneShot(laserShootSound, 0.2f);
        target.GetComponent<Rigidbody2D>().gravityScale = -1.5f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(reportTo.getFingerPosition().x, reportTo.getFingerPosition().y, -14));
        lineRenderer.SetPosition(1, new Vector3(target.transform.position.x, target.transform.position.y, -14));
        if (particleEffect == null)
        {
            particleEffect = Instantiate(hitParticlePrefab, new Vector3(target.transform.position.x, target.transform.position.y, -14), target.transform.rotation);
        }
        else
        {
            particleEffect.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -14);
        }
        target.transform.Rotate(0, 0, 200 * Time.deltaTime);
        lineRenderer.enabled = true;
        GameObject destroyed = fruit.reduceHealth(damageValue * Time.deltaTime, blade, collider);

    }

    public void targetDestroyed()
    {
        selfAudioSource.Stop();
        if (reportTo != null)
        {
            reportTo.laserIsVancant(this.gameObject);
        }
        lineRenderer.enabled = false;
        if (particleEffect != null)
        {
            Destroy(particleEffect);
        }
    }

    public void setTarget(IBladeSpecialAbility reportTo, float damageValue, Blade blade, Collider2D collider)
    {
        this.target = collider.gameObject;
        this.reportTo = reportTo;
        this.damageValue = damageValue;
        this.blade = blade;
        this.collider = collider;
    }

}
