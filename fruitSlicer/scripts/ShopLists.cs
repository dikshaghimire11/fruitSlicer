using UnityEngine;

public class ShopLists : MonoBehaviour
{

    [Header("Shop Data")]
    [HideInInspector]
    public ShopItem[] selectedShopList;

    public ShopItem[] shopBackgroundList;

    public ShopItem[] bladeItemList;

    public GameObject[] characterList;
    public static ShopLists instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
      if (instance == null) { instance = this; } 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
      public  void LoadSavedData()
    {
        for (int i = 0; i < selectedShopList.Length; i++)
        {
            Debug.Log(selectedShopList[i].itemName);
            int sold = PlayerPrefs.GetInt("Sold_" + i, selectedShopList[i].isDefault ? 1 : 0);
            selectedShopList[i].isPurchased = (sold == 1);
            Debug.Log("Sold" + sold);
            int equippedID = PlayerPrefs.GetInt("Equipped_" + selectedShopList[i].itemType, -1);
            Debug.Log("Equipped: " + equippedID);
            if (equippedID == -1 && selectedShopList[i].isDefault && selectedShopList[i].itemType == ShopItem.ItemType.Blade) equippedID = i;

            selectedShopList[i].isEquipped = (equippedID == i);
        }
    }
}
