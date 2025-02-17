using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardContainerState : INetworkSerializable, IEquatable<LeaderboardContainerState>
{
    public ulong clientId;
    public FixedString32Bytes PlayerName;
    public int Coins;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderboardContainerState state)
    {
        return clientId == state.clientId &&
            PlayerName.Equals(state.PlayerName) &&
            Coins == state.Coins;
    }
}
