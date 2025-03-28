using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaManagers : MonoBehaviour
{
    public GameObject buyMapPanel;
    public Button confirmButton;
    public Button cancelButton;
    public Text mapText;
    public int score = 0;
    public Text scoreText;

    public Image[] mapAreas; // Array for map areas (drag images in Inspector)
    private string selectedMapKey = ""; // Store which map area is being purchased
    public void OpenMapPanel1() { MapPanel("MapArea_1", 0); }
    public void OpenMapPanel2() { MapPanel("MapArea_2", 1); }
    public void OpenMapPanel3() { MapPanel("MapArea_3", 2); }
    public void OpenMapPanel4() { MapPanel("MapArea_4", 3); }
    public void OpenMapPanel5() { MapPanel("MapArea_5", 4); }
    public void OpenMapPanel6() { MapPanel("MapArea_6", 5); }
    public void OpenMapPanel7() { MapPanel("MapArea_7", 6); }

    private void Start()
    {
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        scoreText.text = score.ToString();
        buyMapPanel.SetActive(false);

        cancelButton.onClick.AddListener(CloseMapPanel);
        confirmButton.onClick.AddListener(ConfirmMapPurchase);

        // Load map area states
        UpdateMapAreas();
    }

    public void CloseMapPanel()
    {
        buyMapPanel.SetActive(false);
    }

    // Open the buy panel for a specific map
    public void MapPanel(string mapKey, int mapIndex)
    {
        selectedMapKey = mapKey;

        // Check if already purchased
        if (PlayerPrefs.GetInt(mapKey, 0) == 1)
        {
            Debug.Log(mapKey + " is already unlocked! Loading trivia...");
            LoadTriviaScene(mapKey); // Go directly to trivia
            return;
        }

        // Otherwise, show the buy panel
        buyMapPanel.SetActive(true);
        mapText.text = "Use 5 points to unlock the area?";
    }


    public void ConfirmMapPurchase()
    {
        if (selectedMapKey == "") return;

        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        if (currentScore >= 5)
        {
            currentScore -= 5;
            PlayerPrefs.SetInt("PlayerScore", currentScore);
            PlayerPrefs.SetInt(selectedMapKey, 1); // Save the unlocked map
            PlayerPrefs.Save();

            if (DisplayScore.instance != null)
            {
                DisplayScore.instance.UpdateScoreUI();
            }

            UpdateMapAreas(); // Update transparency for unlocked maps
            CloseMapPanel();


            // Navigate to trivia scene based on the selected map
            LoadTriviaScene(selectedMapKey);
        }
        else
        {
            mapText.text = "Not enough points to unlock this! Please play to gain points!";
            Debug.Log("Not enough points to unlock the map!");
        }
    }
    void LoadTriviaScene(string mapKey)
    {
        string sceneName = "";

        switch (mapKey)
        {
            case "MapArea_1": sceneName = "TriviaScene1"; break;
            case "MapArea_2": sceneName = "TriviaScene2"; break;
            case "MapArea_3": sceneName = "TriviaScene3"; break;
            case "MapArea_4": sceneName = "TriviaScene4"; break;
            case "MapArea_5": sceneName = "TriviaScene5"; break;
            case "MapArea_6": sceneName = "TriviaScene6"; break;
            case "MapArea_7": sceneName = "TriviaScene7"; break;
            default: Debug.Log("No trivia scene found!"); return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Check purchased maps and update visibility
    private void UpdateMapAreas()
    {
        for (int i = 0; i < mapAreas.Length; i++)
        {
            string key = "MapArea_" + (i + 1);
            bool isUnlocked = PlayerPrefs.GetInt(key, 0) == 1;

            Color color = mapAreas[i].color;
            color.a = isUnlocked ? 1f : 0.4f; // Fully visible if unlocked, transparent if locked
            mapAreas[i].color = color;
        }
    }
}
