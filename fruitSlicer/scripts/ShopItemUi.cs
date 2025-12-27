using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    // Drag these in your Prefab
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText; // Used for Price OR "Select" status
    public TextMeshProUGUI actionButtonText;
    public Image itemIcon;
    public Button actionButton;       // The "Buy" / "Equip" button
    public GameObject coinIcon;       // Hide this when item is bought

    private int myIndexInManager;
    private ShopManager managerRef;

    public void Setup(ShopItem item, int index, ShopManager manager)
    {
        myIndexInManager = index;
        managerRef = manager;

        // 1. Set Visuals
        nameText.text = item.itemName;
        itemIcon.sprite = item.icon;

        // 2. Set Button State logic
        if (item.isEquipped)
        {
            priceText.text = "EQUIPPED";
            coinIcon.SetActive(false);
            actionButton.interactable = false; // Disable button
            actionButton.image.color = Color.gray;
        }
        else if (item.isPurchased)
        {
            priceText.text = "PURCHASED";
            actionButtonText.text="EQUIP";
            coinIcon.SetActive(false);
            actionButton.interactable = true;
            actionButton.image.color = Color.yellow; // Make button look like "Equip"
        }
        else
        {
            priceText.text = item.price.ToString();
            coinIcon.SetActive(true);
            actionButton.interactable = true;
            actionButton.image.color = Color.green; // Make button look like "Buy"
        }

        // 3. Reset Button Listener to avoid duplicates
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        // Tell the manager: "Item #5 was clicked!"
        managerRef.OnItemButtonClicked(myIndexInManager);
    }
}