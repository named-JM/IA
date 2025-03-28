using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public Transform shopContent;             // Parent container for shop items
    public GameObject shopItemPrefab;         // Prefab for shop items
    public Text feedbackText;                 // Text component for displaying feedback messages

    public Sprite[] itemSprites;              // Array of item sprites
    public string[] itemNames;                // Array of item names
    public int[] itemPrices;                  // Array of item prices

    public GameObject[] itemsToActivate;      // Array of GameObjects to activate when purchased

    private void Start()
    {
        for (int i = 0; i < itemNames.Length; i++)
        {
            GameObject itemObject = Instantiate(shopItemPrefab, shopContent);
            ShopItem shopItem = itemObject.GetComponent<ShopItem>();

            // Check if the item is already purchased
            bool isActivated = IsItemActivated(itemNames[i]);

            // Initialize the item and pass the feedback text and reference to StoreManager for activation
            shopItem.InitializeItem(itemNames[i], itemPrices[i], itemSprites[i], feedbackText, this);

            // If the item is purchased, activate it and update the shop UI
            if (isActivated)
            {
                ActivateItem(i); // Activate the item in the scene
                shopItem.DisableItem(); // Disable the shop button for this item
                feedbackText.text = $"Already Purchased: {itemNames[i]}";
            }
        }
    }

    // Method to activate the corresponding item based on the item index
    public void ActivateItem(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < itemsToActivate.Length)
        {
            // Activate the specified GameObject in the scene
            itemsToActivate[itemIndex].SetActive(true);
            Debug.Log($"{itemNames[itemIndex]} activated!");
        }
        else
        {
            Debug.LogWarning("Invalid item index for activation.");
        }
    }

    public bool IsItemActivated(string itemName)
    {
        return PlayerPrefs.GetInt(itemName + "_Activated", 0) == 1; // 0 is the default value, meaning not activated
    }
}
