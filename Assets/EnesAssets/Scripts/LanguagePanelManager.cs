using UnityEngine;

public class LanguagePanelManager : MonoBehaviour
{
    public GameObject languagePanel; // LanguageSelectorPanel atanacak

    private bool isActive = false;

    public void TogglePanel()
    {
        isActive = !isActive;
        languagePanel.SetActive(isActive);
    }

    public void ClosePanel()
    {
        isActive = false;
        languagePanel.SetActive(false);
    }
}
