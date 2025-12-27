using UnityEngine;

[System.Serializable] // Makes it visible in Inspector
public class ShopItem
{
    public string itemName;
    public int price;
    public Sprite icon;          // The icon shown in the shop UI
    public Sprite actualSprite;  // The actual background image or Sword Sprite for the game

    public bool isDefault;
    public enum ItemType { Blade, Background }
    public ItemType itemType;    // Select "Blade" or "Background" in Inspector

    [HideInInspector] public bool isPurchased;
    [HideInInspector] public bool isEquipped;
}