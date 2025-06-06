using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Inspector'dan Ayarlanabilir")]
    public string sceneToLoad = "LoginScene";
    public bool useFade = true;

    [ContextMenu("Sahneyi Yükle")]
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (useFade && SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Debug.LogWarning("SceneLoader: Sahne ismi atanmadı!");
        }
    }

    // Opsiyonel olarak başka scriptlerden çağırmak istersen:
    public static void LoadSceneStatic(string sceneName, bool fade = true)
    {
        if (fade && SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
