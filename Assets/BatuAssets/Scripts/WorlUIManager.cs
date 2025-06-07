using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldUIManager : NetworkBehaviour
{
    public ulong OwnerNetId;
    public Image healthBar;
    public Image shieldBar;


    [SerializeField]private Color damageColor = Color.red;



    private Coroutine healthRoutine;
    private Coroutine shieldRoutine;

    private Color originalHealthColor;
    private Color originalShieldColor;

    private void Awake()
    {
        originalHealthColor = healthBar.color;
        originalShieldColor = shieldBar.color;
    }
    public override void OnNetworkSpawn()
    {
        

        PlayerDataManager.OnAnyHealthChanged += HandleHealthChanged;
        PlayerDataManager.OnAnyShieldChanged += HandleShieldChanged;
    }


    public override void OnNetworkDespawn()
    {
        PlayerDataManager.OnAnyHealthChanged -= HandleHealthChanged;
        PlayerDataManager.OnAnyShieldChanged -= HandleShieldChanged;
    }

    private void HandleHealthChanged(ulong playerId, float current, float max)
    {
        if (playerId != OwnerNetId) return;

        float targetFill = current / max;

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        healthRoutine = StartCoroutine(AnimateBar(healthBar, targetFill, originalHealthColor));

    }

    private void HandleShieldChanged(ulong playerId, float current, float max)
    {
        if (playerId != OwnerNetId) return;

        float targetFill = current / max;

        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        shieldRoutine = StartCoroutine(AnimateBar(shieldBar, targetFill, originalShieldColor));

    }

    private IEnumerator AnimateBar(Image bar, float targetFill, Color originalColor)
    {
        Vector3 originalScale = bar.rectTransform.localScale;

        bar.color = damageColor;

        Vector3 punchScale = originalScale * 1.2f;
        float punchDuration = 0.1f;
        float elapsed = 0f;

        while (elapsed < punchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / punchDuration;
            bar.rectTransform.localScale = Vector3.Lerp(originalScale, punchScale, t);
            yield return null;
        }

        float lerpDuration = 0.4f;
        elapsed = 0f;
        float startFill = bar.fillAmount;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lerpDuration;
            bar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            bar.rectTransform.localScale = Vector3.Lerp(punchScale, originalScale, t);
            bar.color = Color.Lerp(damageColor, originalColor, t);
            yield return null;
        }

        bar.fillAmount = targetFill;
        bar.rectTransform.localScale = originalScale;
        bar.color = originalColor;
    }

}
