using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [HideInInspector] public static int levelsUnlocked = 1;
    public Button[] levelButtons; // Assign in Inspector
    public Image[] lockImages;    // Assign in Inspector

    void Start()
    {
        levelsUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);
        UpdateLevelSelection();
    }

    void UpdateLevelSelection()
    {
        if (levelButtons.Length != lockImages.Length)
        {
            Debug.LogError("Mismatch in levelButtons and lockImages array sizes!");
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < levelsUnlocked)
            {
                levelButtons[i].interactable = true;
                lockImages[i].gameObject.SetActive(false);
                int levelIndex = i + 1;
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                levelButtons[i].interactable = false;
                lockImages[i].gameObject.SetActive(true);
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        string sceneName = "Stage" + levelIndex;
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found! Ensure the scene is added to Build Settings.");
        }
    }

    public void UnlockNextLevel()
    {
        if (levelsUnlocked < levelButtons.Length)
        {
            levelsUnlocked++;
            PlayerPrefs.SetInt("LevelUnlocked", levelsUnlocked);
            PlayerPrefs.Save();
            UpdateLevelSelection();
        }
        else
        {
            Debug.Log("All levels are already unlocked!");
        }
    }

    public static int GetLevelsUnlocked()
    {
        return levelsUnlocked;
    }
}
