using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        foreach (var anim in FindObjectsOfType<UIEntranceAnimator>())
        {
            if (!anim.enabled || !anim.gameObject.activeInHierarchy) continue;

            anim.StartCoroutine(anim.PlayEntrance());
        }
    }
}