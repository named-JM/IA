using UnityEngine;
using UnityEngine.UI;

public class ImageZoomAndPan : MonoBehaviour
{
    public RectTransform imageRect; // Reference to the image RectTransform
    public Slider zoomSlider;       // Slider for zooming (optional, for Editor testing)

    public float minScale = 1f;     // Minimum zoom scale
    public float maxScale = 3f;     // Maximum zoom scale

    private Vector2 dragStartPos;   // Start position for panning
    private Vector2 dragCurrentPos; // Current drag position

    private float previousTouchDistance; // Distance between touches for pinch-to-zoom

    void Start()
    {
        // Initialize zoom slider
        if (zoomSlider != null && imageRect != null)
        {
            zoomSlider.minValue = minScale;
            zoomSlider.maxValue = maxScale;
            zoomSlider.value = minScale;

            zoomSlider.onValueChanged.AddListener(UpdateZoom);
        }
    }

    void Update()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            HandleMouseInput(); // For mouse testing
        }
        else
        {
            HandleTouchInput(); // For mobile touch
        }
    }

    /// <summary>
    /// Updates the zoom level using slider or pinch-to-zoom.
    /// </summary>
    /// <param name="value">Zoom value</param>
    public void UpdateZoom(float value)
    {
        if (imageRect != null)
        {
            imageRect.localScale = new Vector3(value, value, 1f);
        }
    }

    /// <summary>
    /// Handles mouse input for zoom and pan in the editor.
    /// </summary>
    void HandleMouseInput()
    {
        // Zoom using mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float scale = Mathf.Clamp(imageRect.localScale.x + scroll, minScale, maxScale);
            UpdateZoom(scale);

            if (zoomSlider != null)
            {
                zoomSlider.value = scale; // Sync with slider
            }
        }

        // Pan using mouse drag
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            dragStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) // While dragging
        {
            dragCurrentPos = Input.mousePosition;
            Vector2 delta = dragCurrentPos - dragStartPos;

            imageRect.anchoredPosition += delta * Time.deltaTime * 2f; // Adjust speed
            dragStartPos = dragCurrentPos; // Update position
        }
    }

    /// <summary>
    /// Handles touch input for pinch-to-zoom and panning.
    /// </summary>
    void HandleTouchInput()
    {
        if (Input.touchCount == 1) // Single finger for panning
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                dragCurrentPos = touch.position;
                Vector2 delta = dragCurrentPos - dragStartPos;

                imageRect.anchoredPosition += delta * Time.deltaTime * 4f; // Adjust speed
                dragStartPos = dragCurrentPos;
            }
        }
        else if (Input.touchCount == 2) // Two fingers for pinch-to-zoom
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);

            if (previousTouchDistance != 0)
            {
                float delta = currentTouchDistance - previousTouchDistance;
                float scale = Mathf.Clamp(imageRect.localScale.x + delta * 0.005f, minScale, maxScale);

                UpdateZoom(scale);

                if (zoomSlider != null)
                {
                    zoomSlider.value = scale; // Sync with slider
                }
            }

            previousTouchDistance = currentTouchDistance;
        }
        else
        {
            previousTouchDistance = 0; // Reset when no pinch
        }
    }
}
