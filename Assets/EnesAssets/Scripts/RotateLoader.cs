using UnityEngine;

public class RotateLoader : MonoBehaviour
{
    [Tooltip("Döndürme hızı (derece/saniye)")]
    public float speed = 200f;

    void Update()
    {
        // Z ekseni etrafında döndür
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
