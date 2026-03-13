using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Fruit : MonoBehaviour
{
    public GameObject leftHalf;
    public GameObject rightHalf;

    public float sliceForce = 5f;
    public float rotationForce = 20f;

    public AudioClip sliceSound;
    public int points = 10;
    public float missYPosition = -8f;

    private float missXPosition = 1.7f;

    public Color juiceColor;
    public float uniformScale = 1f;

    private bool isSliced = false;

    // Shared AudioSource for all fruits
    private static AudioSource sliceSource;

    public float health = 100;

    public ISpecialAbilityController attackedBy;
    public Boolean attackedByBoolean;

    public SpriteRenderer frozenEffect;

    public FrozenFruitEffect frozenFruitEffect;



    private Rigidbody2D rb;

    void Awake()
    {
        // Cache AudioSource ONCE (no runtime searching during slice)
        if (sliceSource == null)
        {
            GameObject audioObj = GameObject.Find("SliceAudio");
            if (audioObj != null)
            {
                sliceSource = audioObj.GetComponent<AudioSource>();
            }



        }
    }

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        frozenEffect = this.transform.Find("FrozenFruit").GetComponent<SpriteRenderer>();
        frozenEffect.enabled = false;
        frozenFruitEffect = gameObject.GetComponent<FrozenFruitEffect>();
    }

    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;

        // PLAY SOUND INSTANTLY (NO DELAY)
        if (sliceSound != null && sliceSource != null)
        {
            sliceSource.pitch = UnityEngine.Random.Range(1.2f, 1.3f); // optional juicy effect
            sliceSource.PlayOneShot(sliceSound, 0.4f);
        }

        // Hide main fruit
        gameObject.SetActive(false);

        if (frozenEffect.enabled)
        {
            frozenFruitEffect.SpawnSlicedParts(Vector2.up);
        }
        // Spawn halves
        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);
        if (ModeManager.Instance.currentMode == GameMode.Archery)
        {
            Vector3 newScale = transform.localScale * 1f;

            leftInst.transform.localScale = newScale;
            rightInst.transform.localScale = newScale;
        }

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
        rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

        float torque = UnityEngine.Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
        leftRb.AddTorque(torque, ForceMode2D.Impulse);
        rightRb.AddTorque(-torque, ForceMode2D.Impulse);

        // Cleanup
        Destroy(gameObject);
        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }

    void Update()
    {
        if (ModeManager.Instance.currentMode != GameMode.Archery)
        {
            if (transform.position.y < missYPosition)
            {
                fruitCrossedBoundary();
            }
            if (rb.linearVelocity.y > 0)
            {
                gameObject.layer = LayerMask.NameToLayer("Fruits");
            }
            else if (rb.linearVelocity.y < 0)
            {
                gameObject.layer = LayerMask.NameToLayer("FruitsDown");
            }
        }
        else
        {
            if (transform.position.x > missXPosition)
            {
                fruitCrossedBoundary();
            }
        }

    }


    public void fruitCrossedBoundary()
    {
        if (attackedBy != null)
        {
            attackedBy.fruitDestroyed(this);
        }
        if (ScoreManager.instance.dontLooseLife)
        {
            return;
        }
        if (ScoreManager.instance != null && ScoreManager.instance.isGameOver)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject);
        switch (ModeManager.Instance.currentMode)
        {
            case GameMode.Infinite:
            case GameMode.Archery:
                if (ScoreManager.instance != null)
                    ScoreManager.instance.LoseLife();
                if (SoundManager.instance != null)
                    SoundManager.instance.PlayMissTargetedFruitSound();
                break;

            case GameMode.JuiceMaking:
                bool isTargetFruit =
        JuiceManager.instance != null &&
        JuiceManager.instance.targetFruitNew != null &&
        gameObject.name.StartsWith(JuiceManager.instance.targetFruitNew.name);
                if (isTargetFruit)
                {
                    if (SoundManager.instance != null)
                    {
                        SoundManager.instance.PlayMissTargetedFruitSound();
                    }
                }
                break;


        }
    }

    public void reduceHealth(float reduceBy, Blade blade, Collider2D collider)
    {
        health = health - reduceBy;
        if (health <= 0)
        {
            blade.destroyFruit(this, collider);
        }
    }
}

