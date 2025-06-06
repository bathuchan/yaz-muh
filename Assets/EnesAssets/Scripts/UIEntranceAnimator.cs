using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIEntranceAnimator : MonoBehaviour
{
    public enum Direction { Left, Right, Top, Bottom }

    [Header("Giriş Ayarları")]
    public Direction fromDirection = Direction.Left;
    public float distance = 500f;
    public float duration = 0.5f;
    public float delay = 0f;
    public bool playOnStart = true;

    [Header("Ses Efekti Ayarları")]
    public AudioClip entranceSound;
    public float volume = 1f;
    public AudioSettingsManager audioSettings;

    private Vector3 targetPosition;
    private LayoutElement layoutElement;
    private bool hasPlayed = false;

    void Awake()
    {
        targetPosition = transform.localPosition;

        layoutElement = GetComponent<LayoutElement>();
        if (layoutElement != null)
            layoutElement.ignoreLayout = true;
    }

    void Start()
    {
        if (playOnStart)
        {
            // Play'i sahneye geçiş sonrası garantiye al
            Invoke(nameof(SafePlay), 0.01f);
        }
    }

    void SafePlay()
    {
        if (!hasPlayed)
        {
            hasPlayed = true;
            StartCoroutine(PlayEntrance());
        }
    }

    public IEnumerator PlayEntrance()
    {
        Vector3 startPos = targetPosition;

        switch (fromDirection)
        {
            case Direction.Left:   startPos += Vector3.left * distance; break;
            case Direction.Right:  startPos += Vector3.right * distance; break;
            case Direction.Top:    startPos += Vector3.up * distance; break;
            case Direction.Bottom: startPos += Vector3.down * distance; break;
        }

        transform.localPosition = startPos;

        yield return new WaitForSeconds(delay);

        // 🔊 Ses efekti
        if (entranceSound != null && audioSettings != null)
        {
            float sfxVol = audioSettings.masterVolume * audioSettings.sfxVolume;
            AudioSource.PlayClipAtPoint(entranceSound, Camera.main.transform.position, sfxVol * volume);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
            yield return null;
        }

        transform.localPosition = targetPosition;

        if (layoutElement != null)
            layoutElement.ignoreLayout = false;
    }
}
