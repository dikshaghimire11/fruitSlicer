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
        // throw new System.NotImplementedException();
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
        abilityUI.chargedEffectParent.SetActive(false);
        abilityUI.readyToAttackButton.SetActive(false);
        abilityUI.attacking.SetActive(false);
        abilityUI.coolingDownButton.SetActive(false);
        // abilityUI.quickChargeButton.GetComponent<Button>().onClick.AddListener(() => purchaseQuickCharge());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
