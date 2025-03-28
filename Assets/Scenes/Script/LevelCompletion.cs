using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour
{
    public GameObject completionPanel;

    // Called when the stage is completed
    public void OnStageComplete()
    {
        // Get the current level number
        int currentLevel = GetCurrentLevel();

        // Unlock the next level
        UnlockNextLevel(currentLevel);

        // Load the level selection scene
        SceneManager.LoadScene("LevelSelection");
        completionPanel.SetActive(false);
    }

    // Unlock the next level based on the current level
    private void UnlockNextLevel(int currentLevel)
    {
        // Check if the next level needs to be unlocked
        if (currentLevel >= LevelSelectionManager.levelsUnlocked)
        {
            LevelSelectionManager.levelsUnlocked = currentLevel + 1;
            PlayerPrefs.SetInt("LevelUnlocked", LevelSelectionManager.levelsUnlocked); // Save progress
            PlayerPrefs.Save();
        }
    }

    private int GetCurrentLevel()
    {
        // Extract the current level number from the scene name
        string currentScene = SceneManager.GetActiveScene().name;

        // Assuming scene names are in the format "Stage1", "Stage2", etc.
        if (currentScene.StartsWith("Stage"))
        {
            if (int.TryParse(currentScene.Replace("Stage", ""), out int levelNumber))
            {
                return levelNumber;
            }
            else
            {
                Debug.LogError("Failed to parse level number from scene name.");
            }
        }
        else
        {
            Debug.LogError("Scene name does not match the expected format 'StageX'.");
        }

        // Fallback in case of an error
        return 1;
    }
}
