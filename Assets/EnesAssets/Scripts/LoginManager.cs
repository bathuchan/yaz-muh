using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("Giriş Alanları")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("Uyarı Paneli")]
    public GameObject warningPanel;

    [Header("Doğru Girişten Sonra Açılacak Sahne")]
    public string successSceneName = "Dashboard";

    private string validUsername = "admin";
    private string validPassword = "admin123";

    public void AttemptLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (username == validUsername && password == validPassword)
        {
            // Başarılı giriş → sahneye geç
            SceneManager.LoadScene(successSceneName);
        }
        else
        {
            // Başarısız → uyarı göster
            if (warningPanel != null)
                warningPanel.SetActive(true);
        }
    }
}
 