using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] int ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
