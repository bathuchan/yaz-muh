using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;
using LiteNetLib.Utils;



public class RegisterManager : MonoBehaviour
{
    [Header("Kayıt Alanları")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("Uyarı Paneli")]
    public GameObject warningPanel;
    public TMP_Text warningText;

    [Header("Başarı Paneli")]
    public GameObject successPanel;
    public TMP_Text successText;

    [Header("Başarılı Kayıttan Sonra Açılacak Sahne")]
    public string successSceneName = "MainMenuScene";

    public bool pendingFlag = false;

    public void AttemptRegister()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim().ToLower();
        string password = passwordInput.text;

        // 🔴 Boş alan kontrolü
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowWarning("Hiçbir alan boş bırakılamaz!");
            return;
        }

        // 🔵 Kullanıcı adı kontrolü
        if (username.Length < 2 || username.Length > 16)
        {
            ShowWarning("Kullanıcı adı 2 ile 16 karakter arasında olmalıdır.");
            return;
        }

        // 🔵 E-posta kontrolü
        if (!IsValidEmail(email))
        {
            ShowWarning("Geçerli bir e-posta adresi giriniz.");
            return;
        }

        // 🔵 Şifre kontrolü
        if (password.Length < 8)
        {
            ShowWarning("Şifre en az 8 karakter olmalıdır.");
            return;
        }


        if (GeneralServerComm.Instance != null)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)CMD_STATUS.CMD_REGISTER);
            writer.Put(username);
            writer.Put(password);
            writer.Put(email);

            GeneralServerComm.Instance.dataFlag = false;
            GeneralServerComm.Instance.sendToPeer(writer);
            pendingFlag = true;

        }
        else
        {
            Debug.Log("SERVER CONNECTION IS NOT CREATED.");
        }



        // ✅ Tüm kontroller geçti → başarı paneli göster, sahneye yönlendir
        //ShowSuccessAndRedirect();
    }

    private void ShowWarning(string message)
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            if (warningText != null)
                warningText.text = message;
        }
    }

    private void ShowSuccessAndRedirect()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(true);
            if (successText != null)
                successText.text = "Kayıt başarılı!\n2 saniye içinde giriş sayfasına yönlendirileceksiniz...";
        }

        StartCoroutine(RedirectAfterDelay(3f));
    }

    private IEnumerator RedirectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(successSceneName);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public void Update()
    {
        if (pendingFlag && GeneralServerComm.Instance.dataFlag)
        {
            pendingFlag = false;
            GeneralServerComm.Instance.dataFlag = false;
            int ret = GeneralServerComm.Instance.return_code;
            if (ret == (int)CMD_STATUS.RET_SUCCESSFUL)
            {
                ShowSuccessAndRedirect();
            }
            else
            {
                Debug.Log($"REGISTER FAILED, RETURN CODE: {ret}");
            }
        }
    }
}
