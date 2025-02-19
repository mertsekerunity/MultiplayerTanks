using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Health health;
    [SerializeField] BountyCoin bountyCoin;

    [Header("Settings")]
    [SerializeField] float coinSpread = 3f;
    [SerializeField] float bountyPercentageNormalized = 0.5f;
    [SerializeField] int bountyCoinCount = 10; //dropped coins
    [SerializeField] int minCoinBountyValue = 10; // min value of dropped coins
    [SerializeField] LayerMask layerMask;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    Collider2D[] coinBuffer = new Collider2D[1];
    float coinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinRadius = bountyCoin.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        health.OnDie -= HandleDie;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!collider.TryGetComponent<Coin>(out Coin coin)) {  return; }
        int coinValue = coin.Collect();

        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * bountyPercentageNormalized);

        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyValue < minCoinBountyValue) return;

        for (int i = 0; i<bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(bountyCoin, GetSpawnPoint(), Quaternion.identity);

            coinInstance.SetValue(bountyCoinValue);

            coinInstance.NetworkObject.Spawn();
        }
    }

    Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            if (numColliders == 0) return spawnPoint;
        }
    }
}
