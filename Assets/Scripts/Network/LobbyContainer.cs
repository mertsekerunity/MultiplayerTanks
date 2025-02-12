using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class LobbyContainer : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] TMP_Text lobbyPlayersText;

    LobbiesList lobbiesList;
    Lobby lobby;

    public void Initialize(LobbiesList lobbiesList, Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;

        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbiesList.JoinAsync(lobby);
    }
}
