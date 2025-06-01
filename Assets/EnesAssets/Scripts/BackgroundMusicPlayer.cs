using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public static BackgroundMusicPlayer Instance;

    [Header("Müzik Ayarları")]
    public AudioClip musicClip;
    public AudioMixerGroup musicMixerGroup; // AudioSettingsManager ile bağlantılı olmalı

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton: sahneler arası birden fazla oluşursa yok et
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = musicMixerGroup;

        audioSource.Play();
    }

    // Opsiyonel: Müzik değiştirmek istersen
    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource.clip == newClip) return;

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
