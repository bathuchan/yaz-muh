
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public ulong OwnerNetId; // this should be assigned on spawn
    public Image healthBar;
    public Image shieldBar;

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
        healthBar.fillAmount = current / max;
    }

    private void HandleShieldChanged(ulong playerId, float current, float max)
    {
        if (playerId != OwnerNetId) return;
        shieldBar.fillAmount = current / max;
    }
}
