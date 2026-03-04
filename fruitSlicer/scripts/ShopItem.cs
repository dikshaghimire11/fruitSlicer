using System;
using UnityEngine;

[System.Serializable] // Makes it visible in Inspector
public class ShopItem
{
    public string itemName;
    public int price;
    public Sprite icon;          // The icon shown in the shop UI
    public Sprite actualSprite;

    public Sprite blurredSprite;

    public GameObject prefb;

    public string description;

    public String specialAbility;



    public bool isDefault;
    public enum ItemType { Blade, Background,Bow }
    public ItemType itemType;    // Select "Blade" or "Background" in Inspector

    [HideInInspector] public bool isPurchased;
    [HideInInspector] public bool isEquipped;
}