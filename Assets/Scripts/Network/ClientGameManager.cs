using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    JoinAllocation allocation;

    const string MenuSceneName = "Main Menu";

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        AuthState currentAuthState = await AuthenticationWrapper.DoAuth();

        if (currentAuthState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }

        catch(Exception ex)
        {
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
