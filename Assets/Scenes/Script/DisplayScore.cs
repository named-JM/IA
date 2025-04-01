using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The DisplayScore class is responsible for managing and displaying the player's score
/// on the user interface. It utilizes the Singleton pattern to ensure only one instance
/// of the score manager exists, allowing easy access from other scripts.
/// </summary>
public class DisplayScore : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText; // Reference to the UI Text component for displaying the score

    public static DisplayScore instance; // Singleton instance of the DisplayScore class

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// This method ensures that there is a single instance of DisplayScore
    /// throughout the game (Singleton pattern).
    /// </summary>
    private void Awake()
    {
        instance = this; // Assign the static instance to this object
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// This method is responsible for initializing the display of the score
    /// when the game starts.
    /// </summary>
    private void Start()
    {
        UpdateScoreUI(); // Call method to update score displayed on UI
    }

    /// <summary>
    /// Updates the score displayed in the UI.
    /// This method retrieves the player's current score from PlayerPrefs
    /// and updates the scoreText UI element.
    /// </summary>
    public void UpdateScoreUI()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0); // Get the player's score, default to 0 if not set

        if (scoreText != null) // Check if the scoreText has been assigned
        {
            scoreText.text = score.ToString(); // Update the scoreText UI with the current score
        }
    }
}