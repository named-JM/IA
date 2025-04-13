using UnityEngine;
using UnityEngine.UI;

public class CanvasColorChanger : MonoBehaviour
{
    public Image panelImage; // Assign the Panel's Image component here

    public void ChangeColor(string colorCode)
    {
        Color newColor;

        // Ensure the color code is prefixed with a '#'
        if (!colorCode.StartsWith("#"))
        {
            colorCode = "#" + colorCode;
        }

        // Try to parse the color code as a hex string
        if (ColorUtility.TryParseHtmlString(colorCode, out newColor))
        {
            panelImage.color = newColor;
        }
        else
        {
            Debug.LogWarning("Invalid color code: " + colorCode);
        }
    }
}