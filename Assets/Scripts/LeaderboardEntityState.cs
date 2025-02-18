using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Coins;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderboardEntityState state)
    {
        return ClientId == state.ClientId &&
            PlayerName.Equals(state.PlayerName) &&
            Coins == state.Coins;
    }
}
