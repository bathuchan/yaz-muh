using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;
using UnityEngine.InputSystem;

public class AbilityJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBase;
    public RectTransform joystickKnob;
    public OnScreenStick onScreenStick;
    public ProjectileData projectileData;
    public int projectileId;

    public PlayerAbility playerAbility;
    public bool wasAiming = false;

    private Finger activeFinger = null;
    private PointerEventData syntheticEventData;
    private bool useEnhancedTouch;

    private bool isDraggingMouse = false;
    private int activePointerId = -1;

    private void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        useEnhancedTouch = false;
#elif UNITY_IPHONE|| UNITY_ANDROID
        useEnhancedTouch = true;

#endif

        if (useEnhancedTouch)
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += HandleFingerDown;
            Touch.onFingerUp += HandleFingerUp;
        }

        syntheticEventData = new PointerEventData(EventSystem.current);
    }

    private void OnDisable()
    {
        if (useEnhancedTouch)
        {
            Touch.onFingerDown -= HandleFingerDown;
            Touch.onFingerUp -= HandleFingerUp;
            EnhancedTouchSupport.Disable();
        }
    }

    private void Start()
    {
        projectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);
    }

    private void Update()
    {
        if (useEnhancedTouch)
        {
            if (activeFinger == null || playerAbility == null) return;

            Vector2 screenPosition = activeFinger.screenPosition;
            syntheticEventData.position = screenPosition;
            onScreenStick.OnDrag(syntheticEventData);
            HandleJoystickLogic(screenPosition);
        }
        else if (isDraggingMouse)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            syntheticEventData.position = screenPosition;
            onScreenStick.OnDrag(syntheticEventData);
            HandleJoystickLogic(screenPosition);
        }
    }

    private void HandleJoystickLogic(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, screenPosition, null, out Vector2 localPoint))
        {
            Vector2 direction = localPoint.normalized;
            float distance = Mathf.Clamp(localPoint.magnitude, 0, onScreenStick.movementRange);
            wasAiming = distance > onScreenStick.movementRange * 0.25f;

            if (playerAbility.projectileId != projectileId) playerAbility.projectileId = projectileId;
            if (playerAbility.currentProjectileData != projectileData) playerAbility.currentProjectileData = projectileData;

            if (wasAiming)
                playerAbility.playerLook.trajectoryManager.StartTrajectory();
            else
                playerAbility.playerLook.trajectoryManager.StopTrajectory();
        }
    }

    // Enhanced Touch Handlers
    private void HandleFingerDown(Finger finger)
    {
        if (activeFinger != null || playerAbility == null) return;

        if (TouchRegistry.IsFingerOwned(finger.index)) return;

        if (!RectTransformUtility.RectangleContainsScreenPoint(joystickBase, finger.screenPosition)) return;

        if (!TouchRegistry.ClaimFinger(finger.index, this)) return;

        activeFinger = finger;
        wasAiming = false;
        playerAbility.projectileId = projectileId;

        syntheticEventData.position = finger.screenPosition;
        onScreenStick.OnPointerDown(syntheticEventData);
    }


    private void HandleFingerUp(Finger finger)
    {
        if (!TouchRegistry.IsFingerOwnedBy(finger.index, this)) return;

        if (wasAiming)
            playerAbility.CastAbility();

        wasAiming = false;
        playerAbility.playerLook.trajectoryManager.StopTrajectory();

        syntheticEventData.position = finger.screenPosition;
        onScreenStick.OnPointerUp(syntheticEventData);

        TouchRegistry.ReleaseFinger(finger.index, this);
        activeFinger = null;
    }

    // PC Mouse Input Handlers
    public void OnPointerDown(PointerEventData eventData)
    {
        if (useEnhancedTouch || playerAbility == null || isDraggingMouse) return;

        isDraggingMouse = true;
        activePointerId = eventData.pointerId;
        wasAiming = false;
        playerAbility.projectileId = projectileId;

        onScreenStick.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Necessary to keep dragging alive, handled in Update()
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (useEnhancedTouch || eventData.pointerId != activePointerId) return;

        isDraggingMouse = false;
        activePointerId = -1;

        onScreenStick.OnPointerUp(eventData);
        playerAbility.playerLook.trajectoryManager.StopTrajectory();

        if (wasAiming)
            playerAbility.CastAbility();

        wasAiming = false;
    }
}
