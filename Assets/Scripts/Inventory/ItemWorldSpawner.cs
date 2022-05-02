using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemWorldSpawner
{
    public static Vector3 RandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
    }
    public Vector3 Position;

    public ItemWorldSpawner(Vector3 v)
    {
        Position = v;
    }
    public void GenerateDropForNPC()
    {
        if (UnityEngine.Random.Range(0f, 1f) > Item.ChanceForNpcItemDrop) return;
        Item item = Item.GetRandomEquipment();
        DropItem(Position, RandomDirection(), item);
        if (UnityEngine.Random.Range(0f, 1f) <= Item.ChanceForNpcHealthPotion)
        {
            DropItem(Position, RandomDirection(), new Item() { itemType = ItemType.HealthPotion, amount = 1 }, false);
        }
        DropItem(Position, RandomDirection(), new Item() { itemType = ItemType.Xp, amount = 5 }, false);
    }
    public void GenerateDropForChest()
    {
        Item item = Item.GetRandomEquipment();
        DropItem(Position, RandomDirection(), item);
        if (UnityEngine.Random.Range(0f, 1f) <= Item.ChanceForNpcHealthPotion)
        {
            DropItem(Position, RandomDirection(), new Item() { itemType = ItemType.HealthPotion, amount = 1 }, false);
        }
        DropItem(Position, RandomDirection(), new Item() { itemType = ItemType.Xp, amount = 5 }, false);
    }
    public void DropGold()
    {
        DropItem(Position, RandomDirection(), new Item() { itemType = ItemType.Coin, amount = 5 }, false);
    }
    private void DropItem(Vector3 dropPos, Vector3 direction, Item item, bool onlyOne = true)
    {
        ItemStructNetcode itemStruct = new ItemStructNetcode();
        itemStruct.SetStruct(item);
        if (onlyOne)
        {
            itemStruct.amount = 1;
        }
        DropItemServerRpc(dropPos, direction, itemStruct);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 dropPos, Vector3 direction, ItemStructNetcode item)
    {
        Vector3 dir = new Vector3(direction.x, direction.y).normalized;
        Vector3 pos = dropPos; // + dir * 0.2f;
        ItemWorld iw = ItemWorld.SpawnItemWorld(pos, new Item() { itemType = item.itemType, amount = item.amount });
        iw.GetComponent<Rigidbody2D>().AddForce(dir * 1.3f, ForceMode2D.Impulse);
        iw.ItemStructNetVar.Value = new ItemStructNetcode(item);
        iw.GetComponent<NetworkObject>().Spawn();
    }
}
