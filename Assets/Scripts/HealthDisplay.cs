using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Health health;
    [SerializeField] Image healthbarImage;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) { return; }

        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0,health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthbarImage.fillAmount = newHealth / health.MaxHealth;
    }
}
