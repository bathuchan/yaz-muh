using UnityEngine;

[CreateAssetMenu(fileName = "UIEffectSettings", menuName = "Settings/UI Effect Settings")]
public class UIEffectSettings : ScriptableObject
{
    [Header("Animasyon Ayarları")]
    public float hoverScale = 0.9f;
    public float scaleSpeed = 10f;

    [Header("Saydamlık")]
    public float clickAlpha = 0.9f;

    [Header("Varsayılan Sesler")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
}
