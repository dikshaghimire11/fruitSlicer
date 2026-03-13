using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class IceEffectController : MonoBehaviour, ISpecialAbilityController

{

    // private GameObject target;
    private List<GameObject> targets = new List<GameObject>();

    // private LineRenderer lineRenderer;

    private IBladeSpecialAbility reportTo;

    private float damageValue;
    private Blade blade;

    public GameObject hitParticlePrefab;

    private GameObject particleEffect;

    private Collider2D collider;

    public AudioClip laserShootSound;

    public AudioSource selfAudioSource;

    public float speed;


    private List<Fruit> fruits = new List<Fruit>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // lineRenderer = gameObject.GetComponent<LineRenderer>();
        // lineRenderer.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (ScoreManager.instance.isGameOver)
        {
            stopAttacking();
        }
        transform.Rotate(0, 0, 200 * Time.deltaTime);
        //         transform.localScale = UnityEngine.Vector3.Lerp(transform.localScale, new UnityEngine.Vector3(0.7f, 0.7f, 0.7f), 1f * Time.deltaTime);
        //         transform.position = UnityEngine.Vector3.MoveTowards(
        //        transform.position,
        //        new UnityEngine.Vector3(0, -3.5f, 0),
        //        speed * Time.deltaTime
        //    );

        // if (targets.Count <= 0)
        // {
        //     // lineRenderer.enabled = false;
        //     return;
        // }
        if (!GameCanvasManager.instance.startSpawning || ScoreManager.instance.isGameOver)
        {
            return;
        }

        // target.GetComponent<Rigidbody2D>().gravityScale = -1.5f;
        // lineRenderer.positionCount = 2;
        // lineRenderer.SetPosition(0, new Vector3(reportTo.getFingerPosition().x, reportTo.getFingerPosition().y, -14));
        // lineRenderer.SetPosition(1, new Vector3(target.transform.position.x, target.transform.position.y, -14));
        // if (particleEffect == null)
        // {
        //     particleEffect = Instantiate(hitParticlePrefab, new Vector3(target.transform.position.x, target.transform.position.y, -14), target.transform.rotation);
        // }
        // else
        // {
        //     particleEffect.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -14);
        // }
        foreach (GameObject target in targets)
        {
            target.transform.Rotate(0, 0, 200 * Time.deltaTime);
        }

        // lineRenderer.enabled = true;
        // fruit.reduceHealth(damageValue * Time.deltaTime, blade, collider);
    }

    public void targetDestroyed(Fruit fruit)
    {

        // if (selfAudioSource.isPlaying)
        // {
        //     selfAudioSource.Stop();
        // }

        // if (reportTo != null)
        // {
        //     reportTo.laserIsVancant(this);
        // }
        // lineRenderer.enabled = false;
        // if (particleEffect != null)
        // {
        //     Destroy(particleEffect);
        // }
        fruits.Remove(fruit);
        targets.Remove(fruit.gameObject);
    }

    public void setTarget(IBladeSpecialAbility reportTo, float damageValue, Blade blade, Collider2D collider, Fruit fruit)
    {
        selfAudioSource.PlayOneShot(laserShootSound, 1);
        fruits.Add(fruit);
        GameObject target = collider.gameObject;
        // target.GetComponent<Rigidbody2D>().gravityScale = -0.7f;
        target.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        fruit.frozenEffect.enabled = true;
        targets.Add(target);
        this.reportTo = reportTo;
        this.damageValue = damageValue;
        this.blade = blade;
        this.collider = collider;
        fruit.attackedBy = this;
        fruit.attackedByBoolean = true;

    }

    public void fruitDestroyed(Fruit fruit)
    {
        targetDestroyed(fruit);
    }

    public void stopAttacking()
    {
        // if (target == null)
        // {
        //     return;
        // }

        foreach (GameObject target in targets)
        {
            target.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        }

        foreach (Fruit fruit in fruits)
        {
            // targetDestroyed(fruit);
            fruit.frozenEffect.enabled = false;
            fruit.attackedBy = null;
            fruit.attackedByBoolean = false;
        }
        GameObject.Destroy(this.transform.parent.gameObject);



    }
}


