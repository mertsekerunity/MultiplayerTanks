using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] Transform lobbyContainerParent;
    [SerializeField] LobbyContainer lobbyContainerPrefab;

    bool isJoining;
    bool isRefreshing;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) { return; }

        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
                new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyContainerParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyContainer lobbyContainer = Instantiate(lobbyContainerPrefab, lobbyContainerParent);
                lobbyContainer.Initialize(this, lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        isRefreshing = false;
    }
    
    public async void JoinAsync(Lobby lobby)
    {
        if(isJoining) return;

        isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        isJoining = false;
    }
}
