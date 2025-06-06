using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InputSystem
{

}




public class PlayerInputSystem
{

    public Vector3 targetPos;
    Camera playerCam;

    public PlayerInputSystem(Camera cam)
    {
        targetPos = Vector3.zero;
        playerCam = cam;

    }


    public void CameraToWorldPointUpdate(Player player)
    {
        RaycastHit hit;
        Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.UseGlobal);
        targetPos = hit.point;
        targetPos.y = player.transform.position.y;
    }


    public void MovementUpdate(Transform character)
    {
       


    }






}
