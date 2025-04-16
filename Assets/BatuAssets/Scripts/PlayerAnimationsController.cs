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
        // Ensure local owner triggers animation
        networkAnimator.SetTrigger("AbilityCast");
    }

    public void UpdateBool(string boolName,bool value)
    {
        // Ensure local owner triggers animation
        networkAnimator.Animator.SetBool(boolName, value);
    }

    public bool GetBool(string boolName)
    {
        return networkAnimator.Animator.GetBool(boolName);
    }
}
