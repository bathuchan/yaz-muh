using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;
using System.Collections.Generic;


public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform panel;
    public RectTransform joystickBase;
    public RectTransform joystickKnob;
    public OnScreenStick onScreenStick;

    private Vector2 originalBasePosition;
    private Finger activeFinger;
    private bool isTouchDragging = false;
    private bool isMouseDragging = false;
    private int activePointerId = -1;
    private bool useEnhancedTouch;

    private void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        useEnhancedTouch = false;
#elif UNITY_IPHONE || UNITY_ANDROID
        useEnhancedTouch = true;
#endif

        if (useEnhancedTouch)
        {
            EnhancedTouchSupport.Enable();


            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerMove += OnFingerMove;
            Touch.onFingerUp += OnFingerUp;
        }

    }

    private void OnDisable()
    {

        if (useEnhancedTouch)
        {
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerMove -= OnFingerMove;
            Touch.onFingerUp -= OnFingerUp;

            if (activeFinger != null)
            {
                TouchRegistry.ReleaseFinger(activeFinger.index, this);
                activeFinger = null;
            }

            EnhancedTouchSupport.Disable();

        }

    }

    private void Start()
    {
        originalBasePosition = joystickBase.anchoredPosition;
    }

    // --- MOBILE TOUCH HANDLING ---
    private void OnFingerDown(Finger finger)
    {

        if (activeFinger != null || isMouseDragging)
        {
            //Debug.Log("Blocked: already dragging or using mouse.");
            return;
        }
        if (TouchRegistry.IsFingerOwned(finger.index))
        {
            //Debug.Log("Blocked: finger already owned.");
            return;
        }

        Vector2 screenPosition = finger.screenPosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(panel, screenPosition, null))
        {
            if (!TouchRegistry.ClaimFinger(finger.index, this))
            {
               // Debug.Log("Blocked: failed to claim finger.");
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, screenPosition, null, out Vector2 localPoint);
            joystickBase.anchoredPosition = localPoint;
            joystickKnob.anchoredPosition = Vector2.zero;

            activeFinger = finger;
            activePointerId = finger.index;
            isTouchDragging = true;
            //Debug.Log("Finger accepted and joystick activated!");

            PointerEventData fakeEvent = new PointerEventData(EventSystem.current) { position = screenPosition, pointerId = activePointerId };
            onScreenStick?.OnPointerDown(fakeEvent);
        }
    }

    private void OnFingerMove(Finger finger)
    {
        if (!isTouchDragging || finger != activeFinger || finger.index != activePointerId) return;

        PointerEventData fakeEvent = new PointerEventData(EventSystem.current) { position = finger.screenPosition, pointerId = activePointerId };
        onScreenStick?.OnDrag(fakeEvent);
    }

    private void OnFingerUp(Finger finger)
    {
        if (!isTouchDragging || finger != activeFinger || finger.index != activePointerId) return;

        isTouchDragging = false;
        TouchRegistry.ReleaseFinger(finger.index, this);
        activeFinger = null;
        activePointerId = -1;

        joystickKnob.anchoredPosition = Vector2.zero;
        joystickBase.anchoredPosition = originalBasePosition;

        PointerEventData fakeEvent = new PointerEventData(EventSystem.current) { position = finger.screenPosition, pointerId = finger.index };
        onScreenStick?.OnPointerUp(fakeEvent);
    }

    // --- EDITOR/PC MOUSE HANDLING ---
    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (activeFinger != null || isTouchDragging || isMouseDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            joystickBase.anchoredPosition = localPoint;
            joystickKnob.anchoredPosition = Vector2.zero;

            isMouseDragging = true;
            activePointerId = eventData.pointerId;

            onScreenStick?.OnPointerDown(eventData);
        }
#endif
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!isMouseDragging || eventData.pointerId != activePointerId) return;

        onScreenStick?.OnDrag(eventData);
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!isMouseDragging || eventData.pointerId != activePointerId) return;

        isMouseDragging = false;
        activePointerId = -1;

        joystickKnob.anchoredPosition = Vector2.zero;
        joystickBase.anchoredPosition = originalBasePosition;

        onScreenStick?.OnPointerUp(eventData);
#endif
    }
}
