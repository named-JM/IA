using UnityEngine;

public class AndroidPinchZoom : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private float zoomSpeed = 0.01f;
    private float minZoom = 0.5f;
    private float maxZoom = 2f;

    private float idleTime = 0f;
    private float resetDelay = 1.5f; // seconds before reset
    private bool isZooming = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (prevTouch0 - prevTouch1).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            float pinchAmount = difference * zoomSpeed;

            Zoom(pinchAmount);

            isZooming = true;
            idleTime = 0f;
        }
        else
        {
            if (isZooming)
            {
                idleTime += Time.deltaTime;
                if (idleTime >= resetDelay)
                {
                    isZooming = false;
                }
            }

            if (!isZooming)
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale, Time.deltaTime * 5f);
            }
        }
    }

    void Zoom(float amount)
    {
        Vector3 newScale = rectTransform.localScale + Vector3.one * amount;
        float clamped = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        rectTransform.localScale = new Vector3(clamped, clamped, 1);
    }
}
