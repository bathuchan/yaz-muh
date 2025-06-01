using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Collections.Generic;

public class LanguageSelector : MonoBehaviour
{
    [System.Serializable]
    public class LanguageOption
    {
        public Button button;
        public GameObject borderImage;
        public RawImage flagImage;
        public TMP_Text languageText;
        public Locale locale;
    }

    public List<LanguageOption> languageOptions;

    private int selectedIndex = -1;

    private void Awake()
    {
        StartCoroutine(InitializeLocaleFromPrefsOrDefault());
    }

    private System.Collections.IEnumerator InitializeLocaleFromPrefsOrDefault()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (PlayerPrefs.HasKey("languageIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("languageIndex");
            if (savedIndex >= 0 && savedIndex < languageOptions.Count)
            {
                LocalizationSettings.SelectedLocale = languageOptions[savedIndex].locale;
                yield break;
            }
        }

        for (int i = 0; i < languageOptions.Count; i++)
        {
            if (languageOptions[i].locale.Identifier.Code == "en")
            {
                LocalizationSettings.SelectedLocale = languageOptions[i].locale;
                PlayerPrefs.SetInt("languageIndex", i);
                PlayerPrefs.Save();
                break;
            }
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("languageIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("languageIndex");
            if (savedIndex >= 0 && savedIndex < languageOptions.Count)
            {
                SelectLanguage(savedIndex);
                return;
            }
        }

        Locale current = LocalizationSettings.SelectedLocale;
        for (int i = 0; i < languageOptions.Count; i++)
        {
            if (languageOptions[i].locale.Identifier == current.Identifier)
            {
                SelectLanguage(i);
                break;
            }
        }
    }

    public void SelectLanguage(int index)
    {
        if (index < 0 || index >= languageOptions.Count) return;

        LocalizationSettings.SelectedLocale = languageOptions[index].locale;
        selectedIndex = index;

        for (int i = 0; i < languageOptions.Count; i++)
        {
            bool isSelected = (i == index);

            // Border görünürlüğü
            if (languageOptions[i].borderImage != null)
                languageOptions[i].borderImage.SetActive(isSelected);

            // TMP Outline ve Dilate ayarı
            if (languageOptions[i].languageText != null)
            {
                Material mat = languageOptions[i].languageText.fontMaterial;

                if (isSelected)
                {
                    mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 1f);
                    mat.SetFloat(ShaderUtilities.ID_FaceDilate, 0.7f); 
                    mat.SetColor(ShaderUtilities.ID_OutlineColor, new Color(39f / 255f, 231f / 255f, 34f / 255f)); // Parlak yeşil
                }
                else
                {
                    mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
                    mat.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
                }
            }

            // Bayrak görselini değiştirme! (istenmiyor)
            // if (languageOptions[i].flagImage != null)
            //     languageOptions[i].flagImage.color = isSelected ? Color.green : Color.white;
        }

        PlayerPrefs.SetInt("languageIndex", index);
        PlayerPrefs.Save();
    }

    public void SelectLanguageByButton(Button clickedButton)
    {
        for (int i = 0; i < languageOptions.Count; i++)
        {
            if (languageOptions[i].button == clickedButton)
            {
                SelectLanguage(i);
                break;
            }
        }
    }
}
