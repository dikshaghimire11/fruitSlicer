using UnityEngine;

public class LaserController : MonoBehaviour, ISpecialAbilityController

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

    private Fruit fruit;
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
            lineRenderer.enabled = false;
            return;
        }
        if (!GameCanvasManager.instance.startSpawning || ScoreManager.instance.isGameOver)
        {
            return;
        }

        if (fruit == null)
            return;

        target.GetComponent<Rigidbody2D>().gravityScale = -0.7f;
        target.GetComponent<Rigidbody2D>().mass += Time.deltaTime;
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
        fruit.reduceHealth(damageValue * Time.deltaTime, blade, collider);
    }

    public void targetDestroyed()
    {
        if (selfAudioSource.isPlaying)
        {
            selfAudioSource.Stop();
        }

        if (reportTo != null)
        {
            reportTo.laserIsVancant(this);
        }
        lineRenderer.enabled = false;
        if (particleEffect != null)
        {
            Destroy(particleEffect);
        }
    }

    public void setTarget(IBladeSpecialAbility reportTo, float damageValue, Blade blade, Collider2D collider, Fruit fruit)
    {
        selfAudioSource.PlayOneShot(laserShootSound, 0.5f);
        this.fruit = fruit;
        this.target = collider.gameObject;
        this.reportTo = reportTo;
        this.damageValue = damageValue;
        this.blade = blade;
        this.collider = collider;
        fruit.attackedBy = this;

    }

    public void fruitDestroyed(Fruit fruit)
    {
        targetDestroyed();
    }

    public void stopAttacking()
    {
        if (target == null)
        {
            return;
        }

        fruit.GetComponent<Rigidbody2D>().gravityScale = 0.3f;
        fruit.attackedBy = null;
        target = null;
        fruit = null;
        targetDestroyed();
    }


}
