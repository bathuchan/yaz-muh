using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class AbilityJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBase; // Background that stays fixed
    public RectTransform joystickKnob; // Moving part of the joystick
    public float joystickRange = 100f;
    public OnScreenStick onScreenStick;
    public ProjectileData projectileData; // Assign projectile data for visualization
    public int projectileId; // Set projectile ID

    private bool isDragging = false;
    public PlayerAbility playerAbility;

    public bool wasAiming = false;

    private void Start()
    {
        projectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);

    }
    private void Update()
    {
        // Continuously update wasAiming

        //Debug.Log("wasAiming:" + wasAiming);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerAbility == null) return;
        isDragging = true;
        wasAiming = false; // Reset here

        playerAbility.projectileId = projectileId;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerAbility == null) return;


        if (!isDragging) return;


        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            Vector2 direction = localPoint.normalized;
            float distance = Mathf.Clamp(localPoint.magnitude, 0, joystickRange);
            //joystickKnob.anchoredPosition = direction * distance;

            wasAiming = distance > joystickRange * 0.25f;

            // Set input for OnScreenStick
            if (wasAiming)
            {
                onScreenStick.OnDrag(eventData);
                playerAbility.playerLook.trajectoryManager.StartTrajectory();


            }
            else
            {
                playerAbility.playerLook.trajectoryManager.StopTrajectory();
            }

            playerAbility.projectileId = projectileId;
            playerAbility.currentProjectileData = projectileData;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerAbility == null) return;

        isDragging = false;
        //joystickKnob.anchoredPosition = Vector2.zero;
        playerAbility.playerLook.trajectoryManager.StopTrajectory();


        if (wasAiming)
        {
            playerAbility.CastAbility();
        }

        wasAiming = false;
    }
}
