using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel;  // The panel that will be shown/hidden

    public void ToggleVisibility()
    {
        // Toggle the panel's active state
        panel.SetActive(!panel.activeSelf);
    }
}
