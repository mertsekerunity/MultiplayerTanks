using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] NetworkObject playerPrefab;

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
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;

        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }
}
