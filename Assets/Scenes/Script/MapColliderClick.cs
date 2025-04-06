using UnityEngine;

public class MapColliderClick : MonoBehaviour
{
    public string mapButtonName; // Like "Button0", "Button3", etc.
    public MapButtonHandler handler; // Reference to the MapButtonHandler script

    void OnMouseDown()
    {
        if (handler != null && !string.IsNullOrEmpty(mapButtonName))
        {
            handler.ShowInfo(mapButtonName); // Call your existing logic
        }
        else
        {
            Debug.LogWarning("Handler or mapButtonName not set for: " + gameObject.name);
        }
    }
}
