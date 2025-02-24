using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] Texture2D crosshair;

    [SerializeField] int ownerPriority = 15;

    [SerializeField] SpriteRenderer minimapIconRenderer;
    [SerializeField] Color ownerColor;
    private CinemachineConfiner2D cinemachineConfiner;


    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet CoinWallet { get; private set; }

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;

            if (IsHost)
            {
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            PlayerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;

            minimapIconRenderer.color = ownerColor;

            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);

            AssignCameraConfiner();
        }
    }

    private void AssignCameraConfiner()
    {
        if (cinemachineConfiner == null)
        {
            cinemachineConfiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        }

        if (cinemachineConfiner != null)
        {
            StartCoroutine(WaitForLevelBounds());
        }
        else
        {
            Debug.LogError("CinemachineConfiner2D not assigned in Player prefab!");
        }
    }

    private IEnumerator WaitForLevelBounds()
    {
        PolygonCollider2D levelBounds = null;

        // Wait until the PolygonCollider2D will be found in the scene
        while (levelBounds == null)
        {
            levelBounds = FindObjectOfType<PolygonCollider2D>();
            if (levelBounds == null)
            {
                //yield return new WaitForSeconds(0.1f); // Wait and retry until it will be found
                //yield return new WaitForEndOfFrame(); // Wait and retry until it will be found
                yield return null; // Wait and retry until it will be found
            }
        }

        // Assign the collider once it's found
        cinemachineConfiner.m_BoundingShape2D = levelBounds;
        cinemachineConfiner.InvalidateCache(); // Ensure the confiner updates
        Debug.Log("CinemachineConfiner2D successfully assigned to the level bounds!");
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}