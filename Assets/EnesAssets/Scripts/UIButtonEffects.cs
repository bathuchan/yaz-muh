using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Bağlantılar")]
    public AudioSettingsManager audioSettings;
    public UIEffectSettings effectSettings;

    private CanvasGroup canvasGroup;
    private AudioSource audioSource;
    private Vector3 originalScale;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioSettings?.audioMixer?.FindMatchingGroups("SFX")?[0];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * effectSettings.hoverScale));

        if (effectSettings.hoverSound)
            audioSource.PlayOneShot(effectSettings.hoverSound, audioSettings.sfxVolume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (effectSettings.clickSound)
            audioSource.PlayOneShot(effectSettings.clickSound, audioSettings.sfxVolume);

        if (canvasGroup != null)
            canvasGroup.alpha = effectSettings.clickAlpha;
    }

    System.Collections.IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * effectSettings.scaleSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
