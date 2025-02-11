using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] RespawningCoin coinPrefab;

    [SerializeField] int maxCoins = 100;
    [SerializeField] int coinValue = 10;
    [SerializeField] Vector2 xSpawnRange;
    [SerializeField] Vector2 ySpawnRange;
    [SerializeField] LayerMask layerMask;

    Collider2D[] coinBuffer = new Collider2D[1];
    float coinRadius;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x,ySpawnRange.y);

            Vector2 spawnPoint = new Vector2(x, y);

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint,coinRadius, coinBuffer, layerMask);

            if(numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
