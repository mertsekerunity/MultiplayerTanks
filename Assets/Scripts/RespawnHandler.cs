using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] Player playerPrefab;
    [SerializeField] float keptCoinPercentageNormalized;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach(Player player in players)
        {
            HandlePlayerSpawned(player);
        }

        Player.OnPlayerSpawned += HandlePlayerSpawned;
        Player.OnPlayerDespawned += HandlePlayerSpawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        Player.OnPlayerSpawned -= HandlePlayerSpawned;
        Player.OnPlayerDespawned -= HandlePlayerSpawned;
    }

    void HandlePlayerSpawned(Player player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }
    
    void HandlePlayerDespawned(Player player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    void HandlePlayerDie(Player player)
    {
        int keptCoins = (int)(player.CoinWallet.TotalCoins.Value * keptCoinPercentageNormalized);

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
    {
        yield return null;

        Player playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        playerInstance.CoinWallet.TotalCoins.Value = keptCoins;
    }
}
