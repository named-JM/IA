using UnityEngine;
using UnityEngine.UI;

public class MapAreaUnlocker : MonoBehaviour
{
    public GameObject modalBuy; // Reference to the modal panel
    public Text coinText; // UI Text to display coins
    public Button buyBtn; // Confirm purchase button
    public Button cancelBtn; // Cancel button
    private int coins;
    private string selectedMap; // To store which map is clicked

    void Start()
    {
        coins = PlayerPrefs.GetInt("coins", 0); // Load coins
        UpdateCoinUI();

        // Ensure modal is hidden at start
        modalBuy.SetActive(false);

        // Assign button listeners
        buyBtn.onClick.AddListener(ConfirmPurchase);
        cancelBtn.onClick.AddListener(CloseModal);
    }

    public void OnMapAreaClick(string mapName)
    {
        selectedMap = mapName; // Store which map was clicked
        modalBuy.SetActive(true); // Show modal
    }

    void ConfirmPurchase()
    {
        if (coins >= 5)
        {
            coins -= 5;
            PlayerPrefs.SetInt("coins", coins); // Save new coin count
            PlayerPrefs.SetInt(selectedMap, 1); // Mark as unlocked
            PlayerPrefs.Save();
            UpdateCoinUI();
            // Load the selected map scene (make sure scenes are added in build settings)
            UnityEngine.SceneManagement.SceneManager.LoadScene(selectedMap);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
        CloseModal();
    }

    void CloseModal()
    {
        modalBuy.SetActive(false);
    }

    void UpdateCoinUI()
    {
        coinText.text = coins.ToString();
    }
}
