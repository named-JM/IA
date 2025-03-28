using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        // Set the default value of the slider
        if (volumeSlider != null)
        {
            volumeSlider.value = 0.5f; // Default volume
            volumeSlider.onValueChanged.AddListener(SetVolume);

            // Set initial volume
            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.SetVolume(volumeSlider.value);
            }
        }
    }

    // Update the background music volume
    void SetVolume(float volume)
    {
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.SetVolume(volume);
        }
    }
}
