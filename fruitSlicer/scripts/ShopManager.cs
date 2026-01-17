using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance; // Singleton for easy access


    // Drag ALL your blades and backgrounds here

    [Header("UI Connections")]
    public Transform contentContainer; // The ScrollView Content
    public GameObject itemPrefab;      // The prefab with ShopItemUI script
    public TextMeshProUGUI coinsText;

    // Track current filter (Blades or Backgrounds)
    
    private int totalCoins;

    void Awake()
    {
          if (instance == null) { instance = this; } 

        // 1. Load Coin Balance (Default 1000 for testing)
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 100);

    }
    void Start()
    {
        UpdateCoinUI();
        ShowBlades();
    }

 

    // --- TAB SYSTEM ---
    public void ShowBlades()
    {
        if(SoundManager.instance!=null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        ShopLists.instance.currentTab = ShopItem.ItemType.Blade;
        ShopLists.instance.selectedShopList = ShopLists.instance.bladeItemList;
        ShopLists.instance.LoadSavedData();
        RefreshUI();
    }

    public void ShowBackgrounds()
    {
         if(SoundManager.instance!=null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
        ShopLists.instance.currentTab = ShopItem.ItemType.Background;
        ShopLists.instance.selectedShopList = ShopLists.instance.shopBackgroundList;
        ShopLists.instance.LoadSavedData();
        RefreshUI();
    }

    // --- MAIN LOGIC ---
    void RefreshUI()
    {
        // 1. Destroy old buttons
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        // 2. Spawn new buttons for the current tab
        for (int i = 0; i < ShopLists.instance.selectedShopList.Length; i++)
        {
            // Filter: Only show items matching the current tab
            // if (selectedShopList[i].itemType == currentTab)
            // {
            GameObject newCard = Instantiate(itemPrefab, contentContainer);
            // Pass 'i' as the ID so we know which item inside 'allItems' array to refer to
            newCard.GetComponent<ShopItemUI>().Setup(ShopLists.instance.selectedShopList[i], i, this);
            // }
        }
    }

    public void OnItemButtonClicked(int id)
    {
        ShopItem item = ShopLists.instance.selectedShopList[id];

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
        ShopItem item = ShopLists.instance.selectedShopList[id];

        if (totalCoins >= item.price)
        {
            // Transaction
            totalCoins -= item.price;
            item.isPurchased = true;

            // Save Data
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("Sold_" +item.itemType+"_"+id, 1);

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
        ShopItem newItem = ShopLists.instance.selectedShopList[id];

        // 1. Unequip all others of same type
        for (int i = 0; i < ShopLists.instance.selectedShopList.Length; i++)
        {
            if (ShopLists.instance.selectedShopList[i].itemType == newItem.itemType)
            {
                ShopLists.instance.selectedShopList[i].isEquipped = false;
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