using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [Header("Loading Step'lerin içinde bulunduğu Canvas veya Container")]
    public Transform loadingStepsParent;

    [Header("Dolmuş hali için texture")]
    public Texture filledTexture;

    [Header("Toplam yükleme süresi (saniye)")]
    public float totalLoadingTime = 3f;

    [Header("Yönlendirilecek sahne ismi")]
    public string nextSceneName = "AnaSahne";

    private List<RawImage> loadingSteps = new List<RawImage>();
    private int currentStep = 0;

    void Start()
    {
        if (loadingStepsParent != null)
        {
            loadingSteps.AddRange(loadingStepsParent.GetComponentsInChildren<RawImage>());
        }

        StartCoroutine(SimulateLoading());
    }

    IEnumerator SimulateLoading()
    {
        int totalSteps = loadingSteps.Count;
        float stepTime = totalLoadingTime / totalSteps;

        while (currentStep < totalSteps)
        {
            yield return new WaitForSeconds(stepTime);

            if (loadingSteps[currentStep] != null && filledTexture != null)
            {
                loadingSteps[currentStep].texture = filledTexture;
            }

            currentStep++;
        }

        // Fade geçişli sahne geçişini başlat
        if (!string.IsNullOrEmpty(nextSceneName) && SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Sahne ismi girilmemiş veya SceneFader yok.");
        }
    }
} 
