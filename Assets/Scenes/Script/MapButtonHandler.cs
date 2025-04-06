using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles the interaction with map buttons in the game, including showing information,
/// managing the purchase of maps, and updating the player's score.
/// </summary>
public class MapButtonHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject infoPanel; // Panel that displays trivia about the selected map
    public TextMeshProUGUI infoText; // Text component to show trivia information
    public Button closeButton; // Button to close the trivia panel
    public GameObject buyMapPanel; // Confirmation panel for purchasing a map
    public TextMeshProUGUI mapText; // Text displaying the purchase confirmation message
    public Button confirmButton, cancelButton; // Buttons to confirm or cancel map purchase
    public Text scoreText; // Text UI element to display the player's current score

    private string selectedMapKey = ""; // Key of the currently selected map
    private int score; // Player's score
    public Color unlockedColor = new Color(1f, 1f, 1f, 0.9f); // Slightly translucent white
    public Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f); // Dim gray

    public List<Button> mapButtons = new List<Button>(); // List of buttons for each map

    [Header("Manual Modals")]
    public List<GameObject> manualModals; // Drag 24 modal GameObjects in the inspector

    // Below this is not used now as I used manual modals
    [Header("Map Info List")]
    public List<MapInfoData> mapInfoList = new List<MapInfoData>();
    [Header("Image Content")]
    public Transform imageContainer; // Drag your ImageContainer here
    public GameObject imagePrefab; // Create a simple prefab with just an Image component
    public GameObject textPrefab; // A simple prefab with just a TextMeshProUGUI component

    /// <summary>
    /// Initializes the script and sets up listeners for UI elements.
    /// Retrieves the player's score and updates the score display.
    /// </summary>
    void Start()
    {
        // Assign listeners for the buttons to handle their click events
        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfo);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseMapPanel);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmMapPurchase);

        // Load the player's score from PlayerPrefs and update the score display
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreUI();

        // Hide the purchase panel and info panel initially
        buyMapPanel.SetActive(false);
        infoPanel.SetActive(false);

        // Update colors of buttons based on purchase status
        UpdateButtonColors();

        // Debug log for understanding map key status
        Debug.Log($"Button {selectedMapKey} unlocked: " + PlayerPrefs.GetInt(selectedMapKey, 0));
        for (int i = 0; i < mapButtons.Count; i++)
        {
            string key = "Map" + i;
            Debug.Log($"[INIT] {key} status: {PlayerPrefs.GetInt(key, 0)}");
        }

    }
    void OnMouseDown()
{
    Debug.Log("Map fragment clicked!");
    // Do whatever you want here
}

    // Hide all modals
    private void HideAllModals()
    {
        foreach (GameObject modal in manualModals)
        {
            modal.SetActive(false);
        }
    }

    public void ShowManualModal(int index)
    {
        if (PlayerPrefs.GetInt("Map" + index, 0) == 1)
        {
            HideAllModals(); // Hide others
            if (index >= 0 && index < manualModals.Count)
            {
                manualModals[index].SetActive(true);
            }
        }
        else
        {
            selectedMapKey = "Map" + index;
            buyMapPanel.SetActive(true);
            mapText.text = "Use 300 points to unlock this area?";
        }
    }

    public void HideAllModalsFromButton()
    {
        HideAllModals();
    }

    /// <summary>
    /// Updates the colors of map buttons based on their purchase status.
    /// </summary>
   private void UpdateButtonColors()
{
    foreach (Button btn in mapButtons)
    {
        string buttonName = btn.name;

        // Extract number from the button name like "Button5"
        string numberPart = new string(buttonName.Where(char.IsDigit).ToArray());

        if (!int.TryParse(numberPart, out int index))
        {
            Debug.LogError("Invalid button name: " + buttonName);
            continue;
        }

        string mapKey = "Map" + index;
        bool isUnlocked = PlayerPrefs.GetInt(mapKey, 0) == 1;

        // Debug check to see what is happening
        Debug.Log($"Checking {mapKey}: Unlocked = {isUnlocked}");

        Image[] allImages = btn.GetComponentsInChildren<Image>(true);
        foreach (Image img in allImages)
        {
            img.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }
}


    /// <summary>
    /// Displays the information panel with trivia. Checks if the map is purchased first.
    /// </summary>
    /// <param name="buttonName">The button name that triggered the info display.</param>
    public void ShowInfo(string buttonName)
    {
        selectedMapKey = buttonName;

        // Extract the number part from "ButtonX"
        string numberPart = new string(buttonName.Where(char.IsDigit).ToArray());

        if (!int.TryParse(numberPart, out int index))
        {
            Debug.LogError("Invalid button name: " + buttonName);
            return;
        }

        // Adjust for PlayerPrefs key format if needed (e.g., "Map1" instead of "Button1")
        string mapKey = "Map" + index;

        if (PlayerPrefs.GetInt(mapKey, 0) == 1)
        {
            HideAllModals();

            if (index >= 0 && index < manualModals.Count)
            {
                manualModals[index].SetActive(true);
            }
            else
            {
                Debug.LogWarning("Manual modal index out of range: " + index);
            }
        }
        else
        {
            selectedMapKey = mapKey; // Make sure selectedMapKey uses the correct format
            buyMapPanel.SetActive(true);
            mapText.text = "Use 300 points to unlock this area?";
        }
    }

    public void CloseModal(GameObject modal)
    {
        modal.SetActive(false);
    }

    /// <summary>
    /// Hides the information panel when close button is clicked.
    /// </summary>
    public void HideInfo()
    {
        infoPanel.SetActive(false); // Deactivate the info panel
    }

    /// <summary>
    /// Confirms the purchase of a map if the player has enough points.
    /// </summary>
    public void ConfirmMapPurchase()
    {
        if (string.IsNullOrEmpty(selectedMapKey)) return; // Make sure a map is selected

        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0); // Get current score

        // Check if the player has enough points to purchase the map
        if (currentScore >= 300)
        {
            currentScore -= 300; // Deduct 300 points for the purchase
            PlayerPrefs.SetInt("PlayerScore", currentScore); // Update PlayerPrefs with new score
            PlayerPrefs.SetInt(selectedMapKey, 1); // Mark the selected map as purchased
            PlayerPrefs.Save(); // Save changes made to PlayerPrefs

            UpdateScoreUI(); // Update the displayed score
            CloseMapPanel(); // Close the buy map panel
            UpdateButtonColors(); // Refresh button colors to reflect purchase

            // Open the trivia panel directly after purchase
            ShowInfo(selectedMapKey);
        }
        else
        {
            // If points are insufficient, show a warning message
            mapText.text = "Not enough points! Play more to earn points.";
            Debug.Log("Not enough points!"); // Log insufficient points warning
        }
    }

    /// <summary>
    /// Closes the buy map confirmation panel.
    /// </summary>
    public void CloseMapPanel()
    {
        buyMapPanel.SetActive(false); // Hide buy map panel
    }

    /// <summary>
    /// Updates the score display on the UI.
    /// </summary>
    private void UpdateScoreUI()
    {
        scoreText.text = PlayerPrefs.GetInt("PlayerScore", 0).ToString(); // Update score text display
    }
   

}

