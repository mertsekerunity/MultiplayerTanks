using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] Transform leaderboardEntityHolder;
    [SerializeField] LeaderboardEntityDisplay leaderboardEntityPrefab;

    NetworkList<LeaderboardEntityState> leaderboardEntities;
    List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntityChanged;

            foreach (LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntityChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

            foreach (Player player in players)
            {
                HandlePlayerSpawned(player);
            }

            Player.OnPlayerSpawned += HandlePlayerSpawned;
            Player.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntityChanged;
        }

        if (IsServer)
        {
            Player.OnPlayerSpawned -= HandlePlayerSpawned;
            Player.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    void HandleLeaderboardEntityChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);

                    leaderboardEntity.Initialize(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);

                    entityDisplays.Add(leaderboardEntity);
                }

                break;

            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                if(displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);

                    Destroy(displayToRemove.gameObject);

                    entityDisplays.Remove(displayToRemove);
                }

                break;

            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                if(displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }

                break;
        }
    }

    void HandlePlayerSpawned(Player player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });

        player.CoinWallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    void HandlePlayerDespawned(Player player)
    {
        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) continue;

            leaderboardEntities.Remove(entity);

            break;
        }

        player.CoinWallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i<leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = leaderboardEntities[i].ClientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                Coins = leaderboardEntities[i].Coins
            };

            return;
        }
    }
}
