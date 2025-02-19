using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Image healPowerBar;

    [Header("Settings")]
    [SerializeField] int maxHealPower = 50;
    [SerializeField] float healCooldown = 60f;
    [SerializeField] int coinsPerTick = 10;
    [SerializeField] int healPerTick = 10;
    [SerializeField] float healTickRate = 1f;

    float remainingCooldown;
    float tickTimer;

    List<Player> playersInZone = new List<Player>();

    NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;

            HandleHealPowerChanged(0,HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer) return;

        if (!collider.attachedRigidbody.TryGetComponent<Player>(out Player player)) return;

        playersInZone.Add(player);

        Debug.Log($"The {player.PlayerName.Value} entered the healing zone");
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!IsServer) return;

        if (!collider.attachedRigidbody.TryGetComponent<Player>(out Player player)) return;

        playersInZone.Remove(player);

        Debug.Log($"The {player.PlayerName.Value} left the healing zone");
    }

    private void Update()
    {
        if (!IsServer) return;

        if(remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;

            if (remainingCooldown <= 0)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;

        if(tickTimer >= 1 / healTickRate)
        {
            foreach (Player player in playersInZone)
            {
                if (HealPower.Value == 0) break;

                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;

                if (player.CoinWallet.TotalCoins.Value < coinsPerTick) continue;

                player.Health.RestoreHealth(healPerTick);
                player.CoinWallet.SpendCoins(coinsPerTick);

                HealPower.Value -= 1;

                if (HealPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }
        }
        tickTimer = tickTimer % (1 / healTickRate);
    }

    void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
