using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
    public Text scoreText;
    public static DisplayScore instance; // Singleton instance

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0);
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
