using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class IceBladeSpecialAbility : MonoBehaviour, IBladeSpecialAbility

{
    public GameObject iceWindPrefab;

    public float coolDownSeconds;

    public float chargingSeconds;

    public int quickChargeCost;

    public float activateFor;

    public Sprite bladeImage;

    public GameObject chargedParticleEffectPrefab;
    public String SpecialAbilityName;
    private bool isCurrentlySlicing;


    private bool readyToAttack;

    private bool isCharging;

    private bool isCoolingDown;

    private Collider2D damageCollider;
    private List<Collider2D> collidedObjects = new List<Collider2D>();

    private List<Collider2D> previousCollided = new List<Collider2D>();

    // private List<ISpecialAbilityController> vacantLazerGameObject = new List<ISpecialAbilityController>();
    // private List<ISpecialAbilityController> allLazerGameObjects = new List<ISpecialAbilityController>();

    private ISpecialAbilityController windGameObject;


    private float tempCoolDownTimer;
    private float tempChargingTimer;

    private float tempAttackFor;

    private Boolean activated;

    public AudioClip abilityUnlockedSound;




    // private List<LineRenderer> occupiedLineRenderer = new List<LineRenderer>();


    // public int totalSimultaneousDamage = 3;

    public float damageValue = 1;

    private Blade blade;

    public Vector3 UIChargeEffectScale;



    public Vector3 position;

    private BladeSpecialAbilityUI abilityUI;

    public void isSlicing(bool value)
    {
        this.isCurrentlySlicing = value;
    }


    public void performAbility()
    {
        collidedObjects.Clear();

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(LayerMask.GetMask("FruitsDown"));

        int count = damageCollider.Overlap(filter, collidedObjects);

        Vector2 origin = position;

        // collidedObjects.Sort((a, b) =>
        // {
        //     float distA = Vector2.Distance(origin, a.transform.position);
        //     float distB = Vector2.Distance(origin, b.transform.position);
        //     return distA.CompareTo(distB);
        // });


        int counter = 0;

        foreach (Collider2D col in collidedObjects)
        {
            if (col == null)
                continue;

            if (col.CompareTag("Bomb") || col.CompareTag("Ice"))
                continue;

            if (ModeManager.Instance.currentMode == GameMode.JuiceMaking && col.gameObject.name != JuiceManager.instance.targetFruitNew.name + "(Clone)")
            {
                continue;
            }
            Fruit fruit = col.gameObject.GetComponent<Fruit>();
            if (fruit == null)
            {
                return;
            }
            if (fruit.attackedBy != null)
            {
                return;
            }

            if (windGameObject.IsUnityNull())
            {
                GameObject wind = Instantiate(iceWindPrefab, iceWindPrefab.transform.position, iceWindPrefab.transform.rotation, GameCanvasManager.instance.transform);
                windGameObject = wind.GetComponentInChildren<ISpecialAbilityController>();
                ParticleSystem ps = wind.GetComponentInChildren<ParticleSystem>(); 
                ps.Simulate(1.5f, true, true);
                ps.Play();
            }

            windGameObject.setTarget(this, damageValue, blade, col, fruit);
            activated = true;

        }




    }

    public void updateFingerPosition(UnityEngine.Vector3 position)
    {
        // this.position = position;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilityUI = GameObject.Find("BladeSpecialAbilityUi").GetComponent<BladeSpecialAbilityUI>();
        abilityUI.bladeImageOnUI.sprite = bladeImage;
        if (SpecialAbilityName != null && !SpecialAbilityName.Equals(""))
        {
            abilityUI.specialAbilityName.text = SpecialAbilityName;
        }
        abilityUI.quickChargeButton.GetComponent<Button>().onClick.AddListener(() => purchaseQuickCharge());
        resetSpecialAbility();
        Transform chargeParentTransform = abilityUI.chargedEffectParent.transform;
        GameObject particleEffect = Instantiate(chargedParticleEffectPrefab, chargeParentTransform.position, chargeParentTransform.rotation, abilityUI.chargedEffectParent.transform);
        particleEffect.transform.localScale = UIChargeEffectScale;
        damageCollider = GameObject.Find("SpecialEffectCollider").GetComponent<Collider2D>();
        blade = gameObject.GetComponent<Blade>();
    }

    public void purchaseQuickCharge()
    {
        if (ScoreManager.instance.score >= quickChargeCost)
        {
            ScoreManager.instance.score -= quickChargeCost;
            bladeReadyToAttack();
        }
        else
        {
            GameCanvasManager.instance.ShowFloatingText("Insuffient Coins!", Color.yellow, abilityUI.quickChargeButton.transform.position, 0.5f, 0f);
            if (SoundManager.instance != null)
                SoundManager.instance.PlayLifeLostSound();
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (!GameCanvasManager.instance.startSpawning || ScoreManager.instance.isGameOver)
        {
            return;
        }
        if (isCharging)
        {
            tempChargingTimer -= Time.deltaTime;
            abilityUI.chargingTimer.text = ((int)tempChargingTimer).ToString();
            if (tempChargingTimer <= 0)
            {
                bladeReadyToAttack();
            }

        }
        else if (isCoolingDown)
        {
            tempCoolDownTimer -= Time.deltaTime;
            abilityUI.coolDownTimer.text = ((int)tempCoolDownTimer).ToString();
            if (tempCoolDownTimer <= 0)
            {
                startCharging();
            }


        }
        else if (readyToAttack)
        {
            performAbility();
            if (activated)
            {
                abilityUI.readyToAttackButton.SetActive(false);
                abilityUI.attacking.SetActive(true);
                tempAttackFor -= Time.deltaTime;
                abilityUI.activationTimer.text = ((int)tempAttackFor).ToString();
                if (tempAttackFor <= 0)
                {
                    activated = false;
                    startCoolingDown();
                }
            }

        }


    }

    public void laserIsVancant(ISpecialAbilityController controller)
    {

        // vacantLazerGameObject.Add(controller);
    }

    public UnityEngine.Vector3 getFingerPosition()
    {
        return position;
    }

    public void resetSpecialAbility()
    {
        abilityUI.coolDownTimer.text = coolDownSeconds.ToString();
        abilityUI.chargingTimer.text = chargingSeconds.ToString();
        abilityUI.quckChargeCost.text = quickChargeCost.ToString();
        startCharging();


    }

    public void startCharging()
    {
        tempChargingTimer = chargingSeconds;
        isCharging = true;
        isCoolingDown = false;
        readyToAttack = false;
        abilityUI.coolingDownButton.SetActive(false);
        if (ModeManager.Instance.currentMode == GameMode.Infinite)
        {
            abilityUI.quickChargeButton.SetActive(true);
        }
        else
        {
            abilityUI.quickChargeButton.SetActive(false);
        }

        abilityUI.recharging.SetActive(true);
        abilityUI.readyToAttackButton.SetActive(false);
        abilityUI.chargedEffectParent.SetActive(false);
        abilityUI.attacking.SetActive(false);
    }

    public void startCoolingDown()
    {
        windGameObject.stopAttacking();
        tempCoolDownTimer = coolDownSeconds;
        isCharging = false;
        isCoolingDown = true;
        readyToAttack = false;
        abilityUI.coolingDownButton.SetActive(true);
        abilityUI.quickChargeButton.SetActive(false);
        abilityUI.recharging.SetActive(false);
        abilityUI.readyToAttackButton.SetActive(false);
        abilityUI.chargedEffectParent.SetActive(false);
        abilityUI.attacking.SetActive(false);

    }
    public void bladeReadyToAttack()
    {

        tempAttackFor = activateFor;
        isCharging = false;
        isCoolingDown = false;
        readyToAttack = true;
        abilityUI.coolingDownButton.SetActive(false);
        abilityUI.quickChargeButton.SetActive(false);
        abilityUI.recharging.SetActive(false);
        abilityUI.readyToAttackButton.SetActive(true);
        abilityUI.chargedEffectParent.SetActive(true);
        abilityUI.attacking.SetActive(false);

        if (SoundManager.instance != null)
        {
            SoundManager.instance.playClip(abilityUnlockedSound);
        }
    }




}
