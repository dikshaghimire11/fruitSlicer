using UnityEngine;

public interface ISpecialAbilityController
{
    public void fruitDestroyed(Fruit fruit);
    public void stopAttacking();
     public void setTarget(IBladeSpecialAbility reportTo, float damageValue, Blade blade, Collider2D collider, Fruit fruit);

}