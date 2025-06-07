using UnityEngine;
using UnityEngine.Localization.Settings;

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        // Localization tamamlandığında yapılacak işlemler
        LocalizationSettings.InitializationOperation.Completed += _ =>
        {
            Debug.Log("✅ Localization initialized.");

            // Localization yüklendikten sonra giriş animasyonlarını çalıştır
            foreach (var anim in FindObjectsOfType<UIEntranceAnimator>())
            {
                if (!anim.enabled || !anim.gameObject.activeInHierarchy) continue;
                anim.StartCoroutine(anim.PlayEntrance());
            }
        };
    }
}
