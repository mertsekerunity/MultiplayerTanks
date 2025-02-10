using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject clientProjectilePrefab;

    [Header("Settings")]
    [SerializeField] float projectileSpeed = 10f;

    bool shouldFire;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        if (!shouldFire) { return; }

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectileClientRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    void PrimaryFireServerRPC(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileIstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileIstance.transform.up = direction;

        SpawnDummyProjectileClientRPC(spawnPos, direction);
    }

    [ClientRpc]
    void SpawnDummyProjectileClientRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) { return;}

        GameObject projectileIstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileIstance.transform.up = direction;
    }
}
