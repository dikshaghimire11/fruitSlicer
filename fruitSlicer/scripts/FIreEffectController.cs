using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class FireEffectController : MonoBehaviour, ISpecialAbilityController

{

    // private GameObject target;
    private List<GameObject> targets = new List<GameObject>();

    // private LineRenderer lineRenderer;

    private IBladeSpecialAbility reportTo;

    private float damageValue;
    private Blade blade;


    private GameObject particleEffect;

    private Collider2D collider;

    public AudioClip affectSoundEffect;

    public AudioSource selfAudioSource;

    public float speed;


    private List<Fruit> fruits = new List<Fruit>();

    private List<Fruit> tempFruits = new List<Fruit>();
    private List<GameObject> tempTargets = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // lineRenderer = gameObject.GetComponent<LineRenderer>();
        // lineRenderer.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        targets.Clear();
        fruits.Clear();
        targets.AddRange(tempTargets);
        fruits.AddRange(tempFruits);
        if (ScoreManager.instance.isGameOver)
        {
            stopAttacking();
        }
        transform.Rotate(0, 0, 200 * Time.deltaTime);
        if (!GameCanvasManager.instance.startSpawning || ScoreManager.instance.isGameOver)
        {
            return;
        }
        foreach (Fruit fruit in fruits)
        {
            // target.transform.Rotate(0, 0, 200 * Time.deltaTime);
            fruit.reduceHealth(damageValue * Time.deltaTime, blade, collider);
        }

    }

    public void targetDestroyed(Fruit fruit)
    {
        tempFruits.Remove(fruit);
        tempTargets.Remove(fruit.gameObject);
    }

    public void setTarget(IBladeSpecialAbility reportTo, float damageValue, Blade blade, Collider2D collider, Fruit fruit)
    {
        selfAudioSource.PlayOneShot(affectSoundEffect, 1);
        tempFruits.Add(fruit);
        GameObject target = collider.gameObject;
        // target.GetComponent<Rigidbody2D>().gravityScale = -0.7f;
        // target.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        // fruit.frozenEffect.enabled = true;
        tempTargets.Add(target);
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


        foreach (Fruit fruit in fruits)
        {
            fruit.attackedBy = null;
            fruit.attackedByBoolean = false;
        }
        GameObject.Destroy(this.transform.parent.gameObject);



    }
}


