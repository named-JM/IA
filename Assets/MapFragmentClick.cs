using UnityEngine;

public class MapFragmentClick : MonoBehaviour
{
    public string buttonName = "Button14"; // Set this in the Inspector (like "Button14")

    private MapButtonHandler mapButtonHandler;

    void Start()
    {
        mapButtonHandler = FindObjectOfType<MapButtonHandler>();
    }

    void OnMouseDown()
    {
        if (mapButtonHandler != null)
        {
            mapButtonHandler.ShowInfo(buttonName);
        }
        else
        {
            Debug.LogWarning("MapButtonHandler not found in the scene.");
        }
    }
}
