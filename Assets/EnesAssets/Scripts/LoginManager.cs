using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using LiteNetLib.Utils;

public class LoginManager : MonoBehaviour
{
    [Header("Giriş Alanları")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("Uyarı Paneli")]
    public GameObject warningPanel;

    [Header("Doğru Girişten Sonra Açılacak Sahne")]
    public string successSceneName = "MainMenuScene";

    private string validUsername = "admin";
    private string validPassword = "admin123";

    public bool pendingFlag = false;

    public void AttemptLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (GeneralServerComm.Instance != null)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)CMD_STATUS.CMD_LOGIN);
            writer.Put(username);
            writer.Put(password);

            GeneralServerComm.Instance.dataFlag = false;
            GeneralServerComm.Instance.sendToPeer(writer);
            pendingFlag = true;

        }
        else
        {
            Debug.Log("SERVER CONNECTION IS NOT CREATED.");
        }

    }


    void Update()
    {
        if (pendingFlag && GeneralServerComm.Instance.dataFlag)
        {
            pendingFlag = false;
            GeneralServerComm.Instance.dataFlag = false;
            int ret = GeneralServerComm.Instance.return_code;
            if (ret == (int)CMD_STATUS.RET_SUCCESSFUL)
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
}
 