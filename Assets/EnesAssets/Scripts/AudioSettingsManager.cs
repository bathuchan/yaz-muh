using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Settings/Audio Settings")]
public class AudioSettingsManager : ScriptableObject
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Exposed Parameter Names")]
    public string masterVolumeParam = "MasterVolume";
    public string sfxVolumeParam = "SFXVolume";
    public string musicVolumeParam = "MusicVolume";
    public string voiceVolumeParam = "VoiceVolume";

    [Header("Volume Levels (0 to 1)")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float voiceVolume = 1f;

    public void ApplyVolumes()
    {
        if (audioMixer == null) return;

        audioMixer.SetFloat(masterVolumeParam, ToDecibel(masterVolume));
        audioMixer.SetFloat(sfxVolumeParam, ToDecibel(masterVolume * sfxVolume));
        audioMixer.SetFloat(musicVolumeParam, ToDecibel(musicVolume));
        audioMixer.SetFloat(voiceVolumeParam, ToDecibel(masterVolume * voiceVolume));
    }

    private float ToDecibel(float value)
    {
        return Mathf.Approximately(value, 0f) ? -80f : Mathf.Log10(value) * 20f;
    }
}
