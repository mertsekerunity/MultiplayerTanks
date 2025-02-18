using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text displayText;

    FixedString32Bytes playerName;

    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId;
        this.playerName = playerName;

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    void UpdateText()
    {
        displayText.text = $"[1] {playerName} - {Coins}";
    }
}
