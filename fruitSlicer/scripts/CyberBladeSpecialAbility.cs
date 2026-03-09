using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class CyberBladeSpecialAbility : MonoBehaviour, IBladeSpecialAbility

{
    public GameObject lazerPrefab;

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

    private List<GameObject> vacantLazerGameObject = new List<GameObject>();

    private float tempCoolDownTimer;
    private float tempChargingTimer;

    private float tempAttackFor;

    private Boolean activated;

    public AudioClip abilityUnlockedSound;


    // private List<LineRenderer> occupiedLineRenderer = new List<LineRenderer>();


    public int totalSimultaneousDamage = 3;

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

        collidedObjects.Sort((a, b) =>
        {
            float distA = Vector2.Distance(origin, a.transform.position);
            float distB = Vector2.Distance(origin, b.transform.position);
            return distA.CompareTo(distB);
        });


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

            if (vacantLazerGameObject.Count <= 0)
            {
                break;
            }
            GameObject vacantLaserGameObject = vacantLazerGameObject[0];
            LaserController controller = vacantLaserGameObject.GetComponent<LaserController>();
            controller.setTarget(this, damageValue, blade, col);
            vacantLazerGameObject.Remove(vacantLaserGameObject);
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
        for (int i = 0; i < totalSimultaneousDamage; i++)
        {
            GameObject lineRenderer = Instantiate(lazerPrefab, transform.position, transform.rotation, transform);
            vacantLazerGameObject.Add(lineRenderer);
        }
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
            SoundManager.instance.PlayLifeLostSound();
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (!GameCanvasManager.instance.startSpawning)
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
                tempAttackFor -= Time.deltaTime;
                if (tempAttackFor <= 0)
                {
                    activated = false;
                    startCoolingDown();
                }
            }

        }


    }

    public void laserIsVancant(GameObject gameObject)
    {
        vacantLazerGameObject.Add(gameObject);
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
    }

    public void startCoolingDown()
    {
        tempCoolDownTimer = coolDownSeconds;
        isCharging = false;
        isCoolingDown = true;
        readyToAttack = false;
        abilityUI.coolingDownButton.SetActive(true);
        abilityUI.quickChargeButton.SetActive(false);
        abilityUI.recharging.SetActive(false);
        abilityUI.readyToAttackButton.SetActive(false);
        abilityUI.chargedEffectParent.SetActive(false);
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
        if (SoundManager.instance != null)
        {
            SoundManager.instance.playClip(abilityUnlockedSound);
        }
    }

}
