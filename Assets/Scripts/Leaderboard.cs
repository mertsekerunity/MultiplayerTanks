using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] Transform leaderboardContainerHolder;
    [SerializeField] LeaderboardContainerDisplay containerDisplayPrefab;

    NetworkList<LeaderboardContainerDisplay> leaderboardContainerDisplays;

    private void Awake()
    {
        leaderboardContainerDisplays = new NetworkList<LeaderboardContainerDisplay>();
    }
}
