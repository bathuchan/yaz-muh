using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Get direction vector from this object to the camera
        Vector3 dirToCam = cam.position - transform.position;

        // Calculate the desired X rotation to face the camera
        float angleX = Quaternion.LookRotation(dirToCam).eulerAngles.x;

        // Apply rotation only on the X axis, keeping Y and Z unchanged
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(angleX, currentRotation.y, currentRotation.z);
    }
}