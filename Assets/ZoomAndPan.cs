using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoomAndPan : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Vector2 lastMousePosition;
    private Vector3 originalScale;
    private Vector2 originalPosition;

    private float zoomSpeed = 0.01f;
    private float minZoom = 0.5f;
    private float maxZoom = 3f;

    private bool isInteracting = false;
    private bool isPinchingThis = false;
    private float resetSpeed = 5f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount < 2) // 🔥 Avoid triggering drag setup during pinch
        {
            lastMousePosition = eventData.position;
            isInteracting = true;
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (isInteracting && Input.touchCount < 2) // 🔥 Only drag if NOT pinching
        {
            Vector2 delta = eventData.position - lastMousePosition;
            rectTransform.anchoredPosition += delta;
            lastMousePosition = eventData.position;
        }
    }



    void Update()
    {
        // Check if this object is being touched
        if (Input.touchCount == 2 && IsTouchOverThisObject())
        {
            isPinchingThis = true;
            isInteracting = true;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (prevTouch0 - prevTouch1).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            float pinchAmount = difference * zoomSpeed;

            Vector3 newScale = rectTransform.localScale + Vector3.one * pinchAmount;
            newScale = ClampScale(newScale);
            rectTransform.localScale = newScale;
        }
        else if (Input.touchCount == 0 && (isInteracting || isPinchingThis))
        {
            // Smoothly return to original scale and position
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale, Time.deltaTime * resetSpeed);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originalPosition, Time.deltaTime * resetSpeed);

            if (Vector3.Distance(rectTransform.localScale, originalScale) < 0.01f &&
                Vector2.Distance(rectTransform.anchoredPosition, originalPosition) < 0.5f)
            {
                rectTransform.localScale = originalScale;
                rectTransform.anchoredPosition = originalPosition;
                isInteracting = false;
                isPinchingThis = false;
            }
        }
#if UNITY_EDITOR
if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
{
    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (Mathf.Abs(scroll) > 0.01f)
    {
        isInteracting = true;
        Vector3 newScale = rectTransform.localScale + Vector3.one * scroll * 5f * zoomSpeed;
        newScale = ClampScale(newScale);
        rectTransform.localScale = newScale;
    }
}
#endif
    
    }

    Vector3 ClampScale(Vector3 scale)
    {
        float clamped = Mathf.Clamp(scale.x, minZoom, maxZoom);
        return new Vector3(clamped, clamped, 1);
    }

    // ✅ Detect if either touch is on this UI image
    bool IsTouchOverThisObject()
    {
        foreach (Touch touch in Input.touches)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = touch.position;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == gameObject)
                    return true;
            }
        }
        return false;
    }
}
