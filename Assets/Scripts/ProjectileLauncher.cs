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
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float fireRate;
    [SerializeField] float muzzleFlashDuration = 0.3f;

    bool shouldFire;
    float previousFireTime;
    float muzzleFlashTimer;

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
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if(muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; }

        if (!shouldFire) { return; }

        if (Time.time < (1/fireRate) + previousFireTime) { return; }

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        previousFireTime = Time.time;
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    void PrimaryFireServerRPC(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            rb2d.velocity = rb2d.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRPC(spawnPos, direction);
    }

    [ClientRpc]
    void SpawnDummyProjectileClientRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) { return;}

        SpawnDummyProjectile(spawnPos, direction);
    }

    void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            rb2d.velocity = rb2d.transform.up * projectileSpeed;
        }
    }
}
