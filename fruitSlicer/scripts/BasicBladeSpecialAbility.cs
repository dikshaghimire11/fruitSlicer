using System;
using UnityEngine;


public class BasicBladeSpecialAbility : MonoBehaviour, IBladeSpecialAbility
{
    private Vector3 empty;
    public Sprite bladeImage;
    public String SpecialAbilityName;

    private BladeSpecialAbilityUI abilityUI;
    public Vector3 getFingerPosition()
    {
        // throw new System.NotImplementedException();
        return empty;
    }

    public void isSlicing(bool value)
    {
        // throw new System.NotImplementedException();
    }

    public void laserIsVancant(ISpecialAbilityController gameObject)
    {
        // throw new System.NotImplementedException();
    }

    public void performAbility()
    {
        // throw new System.NotImplementedException();
    }

    public void resetSpecialAbility()
    {
        abilityUI.scannerNet.SetActive(false);
        abilityUI.chargeSlider.gameObject.SetActive(false);
        abilityUI.chargedEffectParent.SetActive(false);
        abilityUI.readyToAttackButton.SetActive(false);
        abilityUI.attacking.SetActive(false);
        abilityUI.coolingDownButton.SetActive(false);
        Transform chargedEffectTransform = abilityUI.chargedEffectParent.transform;
        if (chargedEffectTransform.childCount > 0)
        {
            Transform chargedEffect = chargedEffectTransform.GetChild(0);
            if (chargedEffect != null)
            {
                GameObject.Destroy(chargedEffect.gameObject);
            }
        }
    }

    public void updateFingerPosition(Vector3 position)
    {
        // throw new System.NotImplementedException();
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
        resetSpecialAbility();
        // abilityUI.quickChargeButton.GetComponent<Button>().onClick.AddListener(() => purchaseQuickCharge());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void stopAttacking()
    {

    }

    public void powerReady()
    {
        // bladeReadyToAttack();
    }

    public float attackTimeLeft()
    {
        return 0f;
    }
}
