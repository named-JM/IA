using UnityEngine;
using UnityEngine.UI;

public class ImageSpawner : MonoBehaviour
{
    public GameObject imagePrefab;  // Drag your Image prefab here
    public Transform imageContainer; // Drag your ImageContainer here
    public Sprite[] sprites; // The array of sprites you want to display

    void Start()
    {
        // Loop through each sprite and instantiate an Image prefab for each
        foreach (Sprite sprite in sprites)
        {
            // Instantiate the Image prefab
            GameObject newImage = Instantiate(imagePrefab, imageContainer);

            // Set the sprite for the Image
            Image imageComponent = newImage.GetComponent<Image>();
            imageComponent.sprite = sprite;
        }
    }
}
