using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    string serverIP;
    int serverPort;
    int queryPort;

    MatchplayBackfiller backfiller;

    NetworkServer networkServer;

    MultiplayAllocationService multiplayAllocationService;

    const string GameSceneName = "Game";

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager networkManager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;

        networkServer = new NetworkServer(networkManager);

        multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

            if (matchmakerPayload != null)
            {
                await StartBackfill(matchmakerPayload);

                networkServer.OnUserJoined += UserJoined;
                networkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogWarning("Matchmaker payload timed out");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error connecting to the server: {ex}");
        }

        if (!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("Server was not able to start successfully");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    async Task StartBackfill(MatchmakingResults payload)
    {
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}",
            payload.QueueName,
            payload.MatchProperties,
            20);

        if (backfiller.NeedsPlayers())
        {
            await backfiller.StartBackfilling();
        }
    }

    void UserJoined(UserData user)
    {
        backfiller.AddPlayerToMatch(user);

        multiplayAllocationService.AddPlayer();

        if (backfiller.NeedsPlayers() && backfiller.IsBackfilling)
        {
            _ = backfiller.StopBackfill();
        }
    }

    void UserLeft(UserData user)
    {
        int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);

        multiplayAllocationService.RemovePlayer();

        if (playerCount <= 0)
        {
            CloseServer();
            return;
        }
    }

    async void CloseServer()
    {
        await backfiller.StopBackfill();
        
        Dispose();

        Application.Quit();
    }

    public void Dispose()
    {
        networkServer.OnUserJoined -= UserJoined;
        networkServer.OnUserLeft -= UserLeft;

        backfiller?.Dispose();

        networkServer?.Dispose();
        multiplayAllocationService?.Dispose();
    }
}
