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
            mapText.text = "Use 300 points to unlock this area?";
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
        if (currentScore >= 300) // Change from 5 to 300
        {
            currentScore -= 300; // Deduct 300 points
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