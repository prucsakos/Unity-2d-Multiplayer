using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct ItemStructNetcode : INetworkSerializable, IEquatable<Item>
{
    public bool isSet;
    public ItemTier itemTier;
    public ItemType itemType;
    public int amount;

    public ItemStructNetcode(Item i)
    {
        isSet = true;
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public ItemStructNetcode(ItemStructNetcode i)
    {
        isSet = i.isSet;
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void SetEmpty()
    {
        isSet = false;
        itemType = ItemType.Pistol;
        itemTier = ItemTier.Common;
        amount = 0;
    }
    public void SetStruct()
    {
        isSet = true;
        itemType = ItemType.Pistol;
        itemTier = ItemTier.Common;
        amount = 0;
    }
    public void SetStruct(ItemType itype, ItemTier itier, int am)
    {
        isSet = true;
        itemType = itype;
        itemTier = itier;
        amount = am;
    }
    public void SetStruct(Item i)
    {
        isSet = true;
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void SetStruct(ItemStructNetcode i)
    {
        isSet = true;
        itemType = i.itemType;
        itemTier = i.itemTier;
        amount = i.amount;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isSet);
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
