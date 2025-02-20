using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    string serverIP;
    int serverPort;
    int queryPort;

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

        if(!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("Server was not able to start successfully");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    public void Dispose()
    {
        networkServer?.Dispose();
        multiplayAllocationService?.Dispose();
    }
}
