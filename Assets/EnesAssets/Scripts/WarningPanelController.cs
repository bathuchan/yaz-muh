using UnityEngine;

public class WarningPanelController : MonoBehaviour
{
    // Bu script hangi panele eklenmişse, onu kapatır
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
