using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class NetworkServer
{
    NetworkManager networkManager;

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);

        UserData userData = JsonUtility.FromJson<UserData>(payload);

        Debug.Log(userData.userName);

        response.Approved = true;

        response.CreatePlayerObject = true;
    }
}
