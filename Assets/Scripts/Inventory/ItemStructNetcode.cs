using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct ItemStructNetcode : INetworkSerializable, IEquatable<Item>
{
    public ItemTier itemTier;
    public ItemType itemType;
    public int amount;

    public ItemStructNetcode(ItemStructNetcode i)
    {
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void SetStruct()
    {
        itemType = ItemType.Pistol;
        itemTier = ItemTier.Common;
        amount = 0;
    }
    public void SetStruct(ItemType itype, ItemTier itier, int am)
    {
        itemType = itype;
        itemTier = itier;
        amount = am;
    }
    public void SetStruct(Item i)
    {
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void SetStruct(ItemStructNetcode i)
    {
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref itemTier);
        serializer.SerializeValue(ref amount);
    }
    public bool Equals(Item other)
    {
        return
            (this.itemType == other.itemType) &&
            (this.itemTier == other.itemTier) &&
            (this.amount == other.amount);
    }
}
