using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform panel; // Parent panel for touch detection
    public RectTransform joystickBase; // The background of the joystick
    public RectTransform joystickKnob; // The moving part of the joystick
    public OnScreenStick onScreenStick; // Reference to Unity's OnScreenStick

    private Vector2 originalBasePosition; // Store the original base position
    private bool isDragging = false;

    void Start()
    {
        // Store the initial base position
        originalBasePosition = joystickBase.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Detect if the touch is inside the panel
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, eventData.position, eventData.pressEventCamera, out Vector2 touchStartPoint))
        {
            // Move joystick base to touch position
            joystickBase.anchoredPosition = touchStartPoint;
            joystickKnob.anchoredPosition = Vector2.zero;
            isDragging = true;

            // Manually send OnPointerDown to OnScreenStick
            if (onScreenStick != null)
            {
                onScreenStick.OnPointerDown(eventData);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Forward drag input to OnScreenStick
        if (onScreenStick != null)
        {
            onScreenStick.OnDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        // Reset joystick knob and base to original position
        joystickKnob.anchoredPosition = Vector2.zero;
        joystickBase.anchoredPosition = originalBasePosition;

        // Forward release input to OnScreenStick
        if (onScreenStick != null)
        {
            onScreenStick.OnPointerUp(eventData);
        }
    }
}
