using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image itemImage;           // Assign the Image component for the item image
    public Text itemNameText;         // Assign the Text component for the item name
    public Text itemPriceText;        // Assign the Text component for the item price
    public Button buyButton;          // Assign the Button component for buying the item
    public Text feedbackText;         // Text component to display feedback messages

    private int itemPrice;
    private string itemName;
    private Sprite itemSprite;

    private StoreManager storeManager; // Reference to StoreManager for activating items

    public void InitializeItem(string name, int price, Sprite image, Text feedback, StoreManager manager)
    {
        itemName = name;
        itemPrice = price;
        itemSprite = image;
        feedbackText = feedback;
        storeManager = manager;

        UpdateUI();
    }

    private void UpdateUI()
    {
        itemNameText.text = itemName;
        itemPriceText.text = $" {itemPrice} points";
        itemImage.sprite = itemSprite;
    }

    public void TryBuyItem()
    {
        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);

        if (currentScore >= itemPrice)
        {
            // Deduct the price from the player's score
            currentScore -= itemPrice;
            PlayerPrefs.SetInt("PlayerScore", currentScore);
            PlayerPrefs.Save();

            // Update the UI display for the score
            UpdateScoreUI();

            // Display success message
            feedbackText.text = $"Purchased: {itemName}!";

            // Save item activation status to PlayerPrefs
            PlayerPrefs.SetInt(itemName + "_Activated", 1); // 1 means activated
            PlayerPrefs.Save();

            // Activate the corresponding item in the scene
            int itemIndex = System.Array.IndexOf(storeManager.itemNames, itemName);
            if (itemIndex >= 0)
            {
                storeManager.ActivateItem(itemIndex);
            }

            // Disable or hide the ShopItem prefab to prevent purchasing again
            DisableItem();
        }
        else
        {
            // Display insufficient points message
            feedbackText.text = "Not enough points to buy this item.";
        }
    }


    private void UpdateScoreUI()
    {
        DisplayScore displayScore = FindObjectOfType<DisplayScore>();
        if (displayScore != null)
        {
            int updatedScore = PlayerPrefs.GetInt("PlayerScore", 0);
            displayScore.scoreText.text = " " + updatedScore.ToString();
        }
    }

    public void DisableItem()
    {
        // Option 1: Disable the ShopItem GameObject
        gameObject.SetActive(false);

        // Option 2: Disable the button to prevent interaction
        // buyButton.interactable = false;
    }

    

}
