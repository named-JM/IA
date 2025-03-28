using UnityEngine;

public class MiniGameSceneManager : MonoBehaviour
{
    void Start()
    {
        // Stop background music and play mini-game music
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMiniGameMusic();
        }
    }
}
