using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    public InputField playerNameInputField;
    public Button submitButton;
    public Text displayText;
    public GameObject panelToClose;
    public Image toggleImage; // Image that will trigger the name input panel

    void Start()
    {
        if (playerNameInputField == null || submitButton == null || displayText == null || panelToClose == null || toggleImage == null)
        {
            Debug.LogError("Please assign all UI components in the Inspector.");
            return;
        }

        // Load and display the saved name on scene load
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            displayText.text = "Hello, " + savedName + "!";
        }
        else
        {
            displayText.text = "Enter your name:";
        }

        submitButton.onClick.AddListener(SaveAndDisplayPlayerName);
    }


    // Toggle the name input panel when the image is clicked
    public void ToggleNameInputPanel()
    {
        // Check if the panel is already active
        bool isActive = panelToClose.activeSelf;

        // Toggle the visibility of the input panel
        panelToClose.SetActive(!isActive);

        // If the panel is being shown, pre-fill the input field with the current saved name
        if (!isActive)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            playerNameInputField.text = savedName;
            playerNameInputField.Select();
            playerNameInputField.ActivateInputField();
        }
    }

    void SaveAndDisplayPlayerName()
    {
        string playerName = playerNameInputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            displayText.text = "Please enter a valid name.";
        }
        else
        {
            // Save the name in PlayerPrefs
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();

            displayText.text = "Hello, " + playerName + "!";
            panelToClose.SetActive(false); // Hide the panel after saving
        }
    }
}
