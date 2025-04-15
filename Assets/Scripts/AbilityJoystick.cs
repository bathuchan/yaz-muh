using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class AbilityJoystick : NetworkBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBase; // Background that stays fixed
    public RectTransform joystickKnob; // Moving part of the joystick
    public float joystickRange = 100f;
    public OnScreenStick onScreenStick;
    public ProjectileData projectileData; // Assign projectile data for visualization
    public int projectileId; // Set projectile ID

    private bool isDragging = false;
    public PlayerAbility playerAbility;

    private void Start()
    {
        if (projectileId == 0)
        {
           // gameObject.SetActive(false); // Disable joystick if no valid ability
        }
        else
        {
            projectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(playerAbility==null || !IsOwner) return;
        isDragging = true;

        playerAbility.projectileId = projectileId;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerAbility == null || ! IsOwner) return;
        if (!isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            Vector2 direction = localPoint.normalized;
            float distance = Mathf.Clamp(localPoint.magnitude, 0, joystickRange);
            joystickKnob.anchoredPosition = direction * distance;

            // Set input for OnScreenStick
            onScreenStick.OnDrag(eventData);

            playerAbility.projectileId = projectileId;
            playerAbility.currentProjectileData = projectileData;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerAbility == null || !IsOwner) return;

        isDragging = false;
        joystickKnob.anchoredPosition = Vector2.zero; // Reset knob position but keep base fixed
        //ResetTrajectory();
    }
}
