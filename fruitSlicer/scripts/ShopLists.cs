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

  [HideInInspector]
  public ShopItem.ItemType currentTab = ShopItem.ItemType.Blade;
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
  public void LoadSavedData()
  {
    for (int i = 0; i < selectedShopList.Length; i++)
    {
      int sold = PlayerPrefs.GetInt("Sold_" +currentTab+"_"+i, selectedShopList[i].isDefault ? 1 : 0);
      selectedShopList[i].isPurchased = (sold == 1);
      int equippedID = PlayerPrefs.GetInt("Equipped_" + currentTab, -1);
      if (equippedID == -1 && selectedShopList[i].isDefault) equippedID = i;

      selectedShopList[i].isEquipped = (equippedID == i);
    }
  }
}
