using UnityEngine;

public class OtherSceneManager : MonoBehaviour
{
    void Start()
    {
        // Play the background music when returning from the mini-game
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayBackgroundMusic();
        }
    }
}
