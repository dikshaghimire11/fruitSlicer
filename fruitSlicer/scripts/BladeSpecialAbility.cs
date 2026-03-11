using System;
using System.Numerics;
using UnityEngine;

public interface IBladeSpecialAbility
{
    public void performAbility();
    public void updateFingerPosition(UnityEngine.Vector3 position);

    public UnityEngine.Vector3 getFingerPosition();

    public void isSlicing(Boolean value);

    public void laserIsVancant(ISpecialAbilityController gameObject);

    public void resetSpecialAbility();

}