using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAnimationsController : NetworkBehaviour
{
    private NetworkAnimator networkAnimator;

    private void Awake()
    {
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    public void TriggerAttackAnimation()
    {
        networkAnimator.Animator.SetTrigger("AbilityCast"); // Still correct
    }

    public void UpdateBool(string boolName, bool value)
    {
        Debug.Log($"[ANIM] Setting {boolName} to {value}");
        networkAnimator.Animator.SetBool(boolName, value); // This is fine
    }

    public bool GetBool(string boolName)
    {
        return networkAnimator.Animator.GetBool(boolName);
    }
}
