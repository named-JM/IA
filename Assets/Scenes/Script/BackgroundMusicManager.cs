using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;  // Singleton instance
    private AudioSource audioSource;

    public AudioClip backgroundMusicClip;  // Background music clip
    public AudioClip miniGameMusicClip;    // Mini-game music clip

    void Awake()
    {
        // Implement Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // Loop background music
        audioSource.playOnAwake = false; // Don't play immediately
        PlayBackgroundMusic();
    }

    // Play the background music
    public void PlayBackgroundMusic()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = backgroundMusicClip;
        audioSource.Play();
    }

    // Play mini-game music
    public void PlayMiniGameMusic()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = miniGameMusicClip;
        audioSource.Play();
    }

    // Stop all music
    public void StopMusic()
    {
        audioSource.Stop();
    }

    // Adjust the music volume
    public void SetVolume(float volume)
    {
        audioSource.volume = volume; // Volume value between 0 and 1
    }
}
