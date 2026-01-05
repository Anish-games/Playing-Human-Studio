using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform rect;
    Rect lastSafeArea = Rect.zero;
    Vector2Int lastScreenSize = Vector2Int.zero;
    ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        ApplySafeArea();
    }

    void Update()
    {
        // Check if safe area or screen has changed
        if (lastSafeArea != Screen.safeArea || 
            lastScreenSize.x != Screen.width || 
            lastScreenSize.y != Screen.height ||
            lastOrientation != Screen.orientation)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;

        // Store current values to detect changes
        lastSafeArea = safe;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastOrientation = Screen.orientation;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;

        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        rect.anchorMin = min;
        rect.anchorMax = max;

        // Debug log to see what's happening (remove after testing)
        Debug.Log($"[SafeArea] Applied - Screen: {Screen.width}x{Screen.height}, SafeArea: {safe}, Anchors: min={min}, max={max}");
    }
}
