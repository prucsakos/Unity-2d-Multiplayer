using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

    public enum ItemType 
    {
        Pistol,
        AR,
        RocketLauncher,
        Head,
        Body,
        HealthPotion,
        Coin,
        Xp
    }
    public enum ItemTier
    {
        Common,
        Good,
        Rare,
        Legendary
    }

public class Item 
{
    public static Item GetRandomEquipment()
    {
        float tierRoll = UnityEngine.Random.Range(0f, 1f);
        float typeRoll = UnityEngine.Random.Range(0f, 1f);
        ItemTier itier;
        ItemType itype;
        int ind = 0;
        while(tierRoll > TierChances[ind])
        {
            ind++;
        }
        itier = TierNames[ind];
        ind = 0;
        while (typeRoll > TypeChances[ind])
        {
            ind++;
        }
        itype = TypeNames[ind];

        return new Item(itype, itier, 1);
    }

    public static float[] TierChances = new float[] { 0.4f, 0.7f, 0.9f, 1f };
    public static ItemTier[] TierNames = new ItemTier[] { ItemTier.Common, ItemTier.Good, ItemTier.Rare, ItemTier.Legendary };

    public static float[] TypeChances = new float[] { 0.25f, 0.35f, 0.5f ,0.75f, 1f };
    public static ItemType[] TypeNames = new ItemType[] { ItemType.Pistol, ItemType.AR, ItemType.RocketLauncher, ItemType.Head, ItemType.Body  };

    public static float ChanceForNpcItemDrop = 0.3f;
    public static float ChanceForNpcHealthPotion = 0.5f;

    public ItemType itemType;
    public ItemTier itemTier;
    public int amount;

    public Item()
    {

    }
    public Item(ItemType itype, ItemTier itier, int am)
    {
        this.itemType = itype;
        this.itemTier = itier;
        this.amount = am;
    }
    public Item(ItemStructNetcode isn)
    {
        this.itemType = isn.itemType;
        this.itemTier = isn.itemTier;
        this.amount = isn.amount;
    }
    public Sprite GetSprite()
    {
        switch(itemType)
        {
            case ItemType.Pistol:    return ItemAssets.Instance.pistolSprite;
            case ItemType.AR:    return ItemAssets.Instance.arSprite;
            case ItemType.RocketLauncher:    return ItemAssets.Instance.rocketLauncherSprite;
            case ItemType.Head:    return ItemAssets.Instance.headSprite;
            case ItemType.Body:   return ItemAssets.Instance.bodySprite;
            case ItemType.HealthPotion:    return ItemAssets.Instance.healthPotionSprite;
            case ItemType.Coin:   return ItemAssets.Instance.coinSprite;
            case ItemType.Xp: return ItemAssets.Instance.xpSprite;
            default: return ItemAssets.Instance.notFound;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Coin:
            case ItemType.HealthPotion:
            case ItemType.Xp:
                return true;
            case ItemType.Pistol:
            case ItemType.AR:
            case ItemType.RocketLauncher:
            case ItemType.Body:
            case ItemType.Head:
                return false;
        }
    }

    public Item clone()
    {
        return new Item() { amount = this.amount, itemType = this.itemType };
    }

}
