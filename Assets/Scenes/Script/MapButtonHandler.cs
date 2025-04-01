using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    public Color lockedColor = Color.gray; // Color for locked (unpurchased) buttons
    public Color unlockedColor = Color.white; // Color for unlocked (purchased) buttons
    public List<Button> mapButtons = new List<Button>(); // List of buttons for each map

    // Dictionary that contains descriptions associated with each map button
    private Dictionary<string, string> mapDescriptions = new Dictionary<string, string>
    {
        { "Button1", "Banaba Lejos" },
        { "Button2", "Calumpang Lejos\n\nResort:\n🏡 Macalima Private Resort" },
        { "Button3", "Daine II" },
        { "Button4", "Tambo Malaki\n\nResort & Accommodation:\nCasa Virliosa Indang\nEMV Flower Farm Main & EMV Villa" },
        { "Button5", "Agus-os\n\nResort:\nEl Herencia Garden Resort\nLa Felicidad Private Resort\nTerre Verte Farm Resort\nResort sa Kubo ni Ising\nLa UlrichLand Farm Resort" },
        { "Button6", "📍 Bancod" },
        { "Button7", "📍 Mataas na Lupa" },
        { "Button8", "Daine I\n\nResort:\nSa Kanluran Private Resort\n\nEvent Place:\nSeven Archangel Church" },
        { "Button9", "Calumpang Cerca\n\nResort:\nCasa GooW Private Farm Resort\nLolo & Lala's Farm\n\nEvent Place:\nEl Silangan Events Place Rental\nDonSyl’s Place Inc." },
        { "Button10", "Alulod\n\nResort:\nEingelzen Private Resort\nR.A's Private Pool\nMojica's Jardin" },
        { "Button11", "Lumampong Balagbag\n\nEco-Tourism:\nCvSU Agri-Eco Tourism Park" },
        { "Button12", "📍 Tambo Kulit" },
        { "Button13", "Kayquit II\n\nResort & Accommodation:\nHacienda Gracita" },
        { "Button14", "Buna Cerca\n\nResort:\nZohlian's Villa\nThe Canopy Farm PH\n\nEvent Place:\nPrecious Garden Events Place\nBuklod Cabin (Glamping & Events)" },
        { "Button15", "Limbon\n\nAccommodation:\nLa Casa de Serenidad\n\nHistorical Site:\nBonifacio Shrine" },
        { "Button16", "Banaba Cerca\n\nResort:\nCasita de Señerez Resort / Carmelita Señerez" },
        { "Button17", "Lumampong Halayhay\n\nResort:\nAlta Rios Resort\n\nEvent Place:\nDayuhan's Events Place\nKanlungan Events Place Rental" },
        { "Button18", "Kaytambog\n\nResort:\nSagana Spring Resort" },
        { "Button19", "📍 Buna Lejos II" },
        { "Button20", "📍 Buna Lejos I" },
        { "Button21", "Guyam Malaki\n\nResort:\nThe Joy of Nicky Private Resort\n\nEvent Place:\nLihim Ng Kubli Farm, Garden, and Events Place\nOur Haven Events Place" },
        { "Button22", "Carasuchi\n\nResort:\nHamani Pool Resort\nHacienda Isabella\nZoila's Private Resort\n\nResort & Event Place:\nLVG Paradise Resort and Events" },
        { "Button23", "Kayquit III\n\nFarm & Resort:\nSanctuario Nature Farms\nSiglo Farm Cafe\nSoiree Private Resort\nSiglo Paraiso" },
        { "Button24", "Mahabang Kahoy Lejos\n\nResort:\nBalay Indang\n\nEvent Place:\nBelle Accueil Events Place\nBalustre Cerca\nDriftwoods Action Park" }
    };

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
    }

    /// <summary>
    /// Updates the colors of map buttons based on their purchase status.
    /// </summary>
    private void UpdateButtonColors()
    {
        foreach (Button btn in mapButtons)
        {
            string buttonName = btn.name; // Get the name of the button
            bool isUnlocked = PlayerPrefs.GetInt(buttonName, 0) == 1; // Check if the map is purchased

            // Set button colors based on unlocked status
            ColorBlock colors = btn.colors;
            colors.normalColor = isUnlocked ? unlockedColor : lockedColor;
            colors.highlightedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.pressedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.selectedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.disabledColor = isUnlocked ? unlockedColor : lockedColor;

            btn.colors = colors; // Apply updated color settings

            // Force button to refresh by temporarily disabling and re-enabling
            btn.interactable = false;
            btn.interactable = true;
        }
    }

    /// <summary>
    /// Displays the information panel with trivia. Checks if the map is purchased first.
    /// </summary>
    /// <param name="buttonName">The button name that triggered the info display.</param>
    public void ShowInfo(string buttonName)
    {
        selectedMapKey = buttonName; // Store the selected map's key

        // Check if the map has been purchased
        if (PlayerPrefs.GetInt(buttonName, 0) == 1)
        {
            // If the area is purchased, show the trivia panel
            infoText.text = mapDescriptions.ContainsKey(buttonName) ? mapDescriptions[buttonName] : "No Information Available.";
            infoPanel.SetActive(true); // Activate the info panel
        }
        else
        {
            // If not purchased, show the buy map panel
            buyMapPanel.SetActive(true);
            mapText.text = "Use 300 points to unlock this area?"; // Display confirmation message
        }
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