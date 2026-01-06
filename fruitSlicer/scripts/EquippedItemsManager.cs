using UnityEngine;
using UnityEngine.UI;

public class EquippedItemsManager : MonoBehaviour
{
    public Image shopBackground;
    public GameObject GameCanvas;



    void Start()
    {
        // --- LOAD BACKGROUND ---
        int bgIndex = PlayerPrefs.GetInt("Equipped_Background", 0); // Default to item 0
                                                                    // Access your ShopManager array or a duplicate list to find sprite
                                                                    // (A better way is to make ShopManager a singleton that doesn't destroy on load)
        shopBackground.sprite = ShopLists.instance.shopBackgroundList[bgIndex].actualSprite;

        int bladeIndex = PlayerPrefs.GetInt("Equipped_Blade", 0);

        GameObject equippedBlade = ShopLists.instance.bladeItemList[bladeIndex].prefb;
        Instantiate(equippedBlade, GameCanvas.transform);

        // --- LOAD BLADE ---
        // int bladeIndex = PlayerPrefs.GetInt("Equipped_Blade", 0);
        // currentBladeRenderer.material.mainTexture = ShopManager.instance.allItems[bladeIndex].actualSprite.texture;
    }
}


