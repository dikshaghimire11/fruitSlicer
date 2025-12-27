using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance; // Singleton for easy access

    [Header("Shop Data")]
    public ShopItem[] allItems; // Drag ALL your blades and backgrounds here

    [Header("UI Connections")]
    public Transform contentContainer; // The ScrollView Content
    public GameObject itemPrefab;      // The prefab with ShopItemUI script
    public TextMeshProUGUI coinsText;
    
    // Track current filter (Blades or Backgrounds)
    private ShopItem.ItemType currentTab = ShopItem.ItemType.Blade;
    private int totalCoins;

    void Awake()
    {
        instance = this;
        
        // 1. Load Coin Balance (Default 1000 for testing)
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);
        UpdateCoinUI();

        // 2. Load Purchased/Equipped Status from PlayerPrefs
        LoadSavedData();
        
        // 3. Show Blades by default
        RefreshUI();
    }

    void LoadSavedData()
    {
        for (int i = 0; i < allItems.Length; i++)
        {
            // Default: First item of each category is owned


            // LOAD PURCHASED: 1 = true, 0 = false
            int sold = PlayerPrefs.GetInt("Sold_" + i, allItems[i].isDefault ? 1 : 0);
            allItems[i].isPurchased = (sold == 1);

            // LOAD EQUIPPED: Get the saved ID for this type
            int equippedID = PlayerPrefs.GetInt("Equipped_" + allItems[i].itemType, -1);
            
            // If nothing saved, equip the first one we find
            if (equippedID == -1 && allItems[i].isDefault && allItems[i].itemType == ShopItem.ItemType.Blade) equippedID = i; 
            
            allItems[i].isEquipped = (equippedID == i);
        }
    }

    // --- TAB SYSTEM ---
    public void ShowBlades()
    {
        currentTab = ShopItem.ItemType.Blade;
        RefreshUI();
    }

    public void ShowBackgrounds()
    {
        currentTab = ShopItem.ItemType.Background;
        RefreshUI();
    }

    // --- MAIN LOGIC ---
    void RefreshUI()
    {
        // 1. Destroy old buttons
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        // 2. Spawn new buttons for the current tab
        for (int i = 0; i < allItems.Length; i++)
        {
            // Filter: Only show items matching the current tab
            if (allItems[i].itemType == currentTab)
            {
                GameObject newCard = Instantiate(itemPrefab, contentContainer);
                // Pass 'i' as the ID so we know which item inside 'allItems' array to refer to
                newCard.GetComponent<ShopItemUI>().Setup(allItems[i], i, this);
            }
        }
    }

    public void OnItemButtonClicked(int id)
    {
        ShopItem item = allItems[id];

        if (item.isPurchased)
        {
            EquipItem(id);
        }
        else
        {
            BuyItem(id);
        }
    }

    void BuyItem(int id)
    {
        ShopItem item = allItems[id];

        if (totalCoins >= item.price)
        {
            // Transaction
            totalCoins -= item.price;
            item.isPurchased = true;

            // Save Data
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("Sold_" + id, 1);

            UpdateCoinUI();
            EquipItem(id); // Auto-equip on buy (optional, keeps UI snappy)
        }
        else
        {
            Debug.Log("Not enough coins!");
            // Optional: Show a "Not enough money" popup
        }
    }

    void EquipItem(int id)
    {
        ShopItem newItem = allItems[id];

        // 1. Unequip all others of same type
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i].itemType == newItem.itemType)
            {
                allItems[i].isEquipped = false;
            }
        }

        // 2. Equip new one
        newItem.isEquipped = true;

        // 3. Save Selection
        // We save "Equipped_Blade" = 5, or "Equipped_Background" = 8
        PlayerPrefs.SetInt("Equipped_" + newItem.itemType, id);
        PlayerPrefs.Save();

        RefreshUI(); // Update texts (Select -> Equipped)
    }

    void UpdateCoinUI()
    {
        coinsText.text = totalCoins.ToString(); // Format as needed (e.g., "1,500")
    }
}