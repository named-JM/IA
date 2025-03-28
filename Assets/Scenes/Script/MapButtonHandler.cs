using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MapButtonHandler : MonoBehaviour
{
    public GameObject infoPanel; // Panel for trivia
    public TextMeshProUGUI infoText; // Trivia text inside the panel
    public Button closeButton; // Button to close trivia panel
    public GameObject buyMapPanel; // Purchase confirmation panel
    public TextMeshProUGUI mapText; // Text inside purchase confirmation panel
    public Button confirmButton, cancelButton; // Purchase buttons
    public Text scoreText; // UI text to display player score

    private string selectedMapKey = "";
    private int score;
    public Color lockedColor = Color.gray;
    public Color unlockedColor = Color.white;
    public List<Button> mapButtons = new List<Button>();



    private Dictionary<string, string> mapDescriptions = new Dictionary<string, string>
    {
    { "Button1", "Banaba Lejos" }, // No location

    { "Button2", "Calumpang Lejos\n🏡 Macalima Private Resort\n📍 Brgy. Calumpang Lejos 1, 4122 Indang, Philippines" },

    { "Button3", "Daine II" },

    { "Button4", "Tambo Malaki\nCasa Virliosa Indang\n📍 III Tambo Malaki, Indang, Cavite\nEMV Flower Farm Main & EMV Villa\n📍 Sitio Portugal, Brgy. Tambo Malaki, Indang, Cavite" },

    { "Button5", "Agus-os\nEl Herencia Garden Resort\n📍 Barangay Agus-os, Indang, Philippines, 4122\nLa Felicidad Private Resort\n📍 Barangay Agus-os, Indang, Philippines, 4122\nTerre Verte Farm Resort\n📍 193 Brgy. Agus-os, Indang, Cavite\nResort sa Kubo ni Ising\n📍 Brgy. Agus-os, Indang, Cavite\nLa UlrichLand Farm Resort\n📍 Barangay Agus-os, Indang, Cavite" },

    { "Button6", "📍Bancod" },

    { "Button7", "📍 Mataas na Lupa" },

    { "Button8", "Daine I\n\nSa Kanluran Private Resort\n📍 East West Road, Brgy. Daine I, Indang, Cavite\nSeven Archangel Church\n📍 Daine, Indang, Cavite" },

    { "Button9", "Calumpang Cerca\n\nCasa GooW Private Farm Resort\n📍 Calumpang Cerca, Indang, Cavite\nLolo & Lala's Farm\n📍 Brgy. Calumpang Cerca, Indang, Cavite\nEl Silangan Events Place Rental\n📍 Purok 4, Brgy. Calumpang Cerca, Indang, Cavite\nDonSyl’s Place Inc.\n📍 Calumpang Cerca, Indang, Cavite" },

    { "Button10", "📍 Alulod\nEingelzen Private Resort\n📍 413 Purok 6, Brgy. Alulod, Indang, Cavite\nR.A's Private Pool\n📍 Patillio Alulod, Indang, Cavite\nMojica's Jardin\n📍 Purok 2, Alulod, Indang, Cavite" },

    { "Button11", "Lumampong Balagbag\n\nCvSU Agri-Eco Tourism Park\n📍 Hostel Tropicana, Indang, Cavite" },

    { "Button12", "📍 Tambo Kulit" }, // No location

    { "Button13", "Kayquit II\n\nHacienda Gracita\n📍 Kayquit, Indang, Cavite, 4121" },

    { "Button14", "Buna Cerca\n\nZohlian's Villa\n📍 Blk 5 Lot 12, Green Valleyfield Subdivision, Buna Cerca, Indang, Cavite\nThe Canopy Farm PH\n📍 Brgy. Buna Cerca, Indang, Philippines, 4122\nPrecious Garden Events Place\n📍 Lot 6005, Buna Cerca Indang Mendez Road, Indang, Philippines\nBuklod Cabin (Glamping & Events)\n📍 Puerto Sampaloc Road, Buna Cerca, Indang, Cavite" },

    { "Button15", "Limbon\n\nLa Casa de Serenidad\n📍 Limbon, Indang, Cavite, Philippines\nBonifacio Shrine\n📍 Brgy. Limbon, Indang, Cavite" },

    { "Button16", "Banaba Cerca\n\nCasita de Señerez Resort / Carmelita Señerez\n📍 Purok 2, Brgy. Banaba Cerca, Indang, Philippines, 4027" },

    { "Button17", "Lumampong Halayhay\n\nAlta Rios Resort\n📍 Brgy. Lumampong Halayhay, Indang-Alfonso Road, Indang, Philippines, 4122\nDayuhan's Events Place\n📍 040 Purok 1, Lumampong Halayhay, Indang, Cavite\nKanlungan Events Place Rental\n📍 Lumampong Halayhay, Indang, Cavite" },

    { "Button18", "Kaytambog\n\nSagana Spring Resort\n📍 4588 Purok 1, Kaytambog, Indang, Cavite" },

    { "Button19", "📍 Buna Lejos II" }, // No location

    { "Button20", "📍 Buna Lejos I" }, // No location

    { "Button21", "Guyam Malaki\n\nThe Joy of Nicky Private Resort\n📍 Purok 7, Barangay Guyam Malaki, Indang, Cavite\nLihim Ng Kubli Farm, Garden, and Events Place\n📍 Purok 7, Ilaya, Guyam Malaki, Indang, 4122 Cavite\nOur Haven Events Place\n📍 Ilaya, Guyam Malaki, Indang, 4122 Cavite" },

    { "Button22", "Carasuchi\nLVG Paradise Resort and Events\n📍 Carasuchi-Anuling St., Mendez, Indang, Cavite\nHamani Pool Resort\n📍 Brgy. Pulo, Indang, Cavite\nHacienda Isabella\n📍 08 Brgy. Carasuchi, Indang, Cavite\nZoila's Private Resort\n📍 Purok 3, Pulo, Indang, Cavite" },

    { "Button23", "Kayquit III\n\nSanctuario Nature Farms\n📍 Indang - Mendez Rd, Sitio Italaro, Indang, 4122 Cavite\nSiglo Farm Cafe\n📍 337 Narra Street, Kayquit 3, Indang, 4122 Cavite\nSoiree Private Resort\n📍 347 Narra, St. Kayquit Road, Indang, Cavite\nSiglo Paraiso\n📍 337 Narra Street, Indang, Cavite" },

    { "Button24", "Mahabang Kahoy Lejos\n\nBalay Indang\n📍 88 Mahabang Kahoy Cerca, Indang, Philippines\nBelle Accueil Events Place\n📍 Mahabang Kahoy Cerca, Indang, Cavite\nBalustre Cerca\n📍 Mahabang Kahoy Cerca, Indang, Cavite\nDriftwoods Action Park\n📍 Brgy. Mahabang Kahoy Cerca, Indang-Mendez Road, 4122 Indang, Cavite" },
};

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfo);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseMapPanel);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmMapPurchase);

        score = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreUI();
        buyMapPanel.SetActive(false);
        infoPanel.SetActive(false);

        UpdateButtonColors(); // Call this to update button colors
        Debug.Log($"Button {selectedMapKey} unlocked: " + PlayerPrefs.GetInt(selectedMapKey, 0));

    }
    private void UpdateButtonColors()
    {
        foreach (Button btn in mapButtons)
        {
            string buttonName = btn.name;
            bool isUnlocked = PlayerPrefs.GetInt(buttonName, 0) == 1;

            ColorBlock colors = btn.colors;
            colors.normalColor = isUnlocked ? unlockedColor : lockedColor;
            colors.highlightedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.pressedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.selectedColor = isUnlocked ? unlockedColor : lockedColor;
            colors.disabledColor = isUnlocked ? unlockedColor : lockedColor;
            btn.colors = colors;

            // Force button to refresh by disabling and re-enabling interactable
            btn.interactable = false;
            btn.interactable = true;
        }
    }



    // Show info but check if the area is purchased first
    public void ShowInfo(string buttonName)
    {
        selectedMapKey = buttonName;

        if (PlayerPrefs.GetInt(buttonName, 0) == 1)
        {
            // If the area is purchased, show the trivia panel
            infoText.text = mapDescriptions.ContainsKey(buttonName) ? mapDescriptions[buttonName] : "No Information Available.";
            infoPanel.SetActive(true);
        }
        else
        {
            // If not purchased, show the buy map panel
            buyMapPanel.SetActive(true);
            mapText.text = "Use 5 points to unlock this area?";
        }
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
    public void ConfirmMapPurchase()
    {
        if (selectedMapKey == "") return;

        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        if (currentScore >= 5)
        {
            currentScore -= 5;
            PlayerPrefs.SetInt("PlayerScore", currentScore);
            PlayerPrefs.SetInt(selectedMapKey, 1); // Mark area as purchased
            PlayerPrefs.Save();

            UpdateScoreUI();
            CloseMapPanel();
            UpdateButtonColors(); // Refresh button colors

            // Now, directly open the trivia panel after purchase
            ShowInfo(selectedMapKey);
        }
        else
        {
            mapText.text = "Not enough points! Play more to earn points.";
            Debug.Log("Not enough points!");
        }
    }

    public void CloseMapPanel()
    {
        buyMapPanel.SetActive(false);
    }

    private void UpdateScoreUI()
    {
        scoreText.text = PlayerPrefs.GetInt("PlayerScore", 0).ToString();
    }
}